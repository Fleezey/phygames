using UnityEngine.Profiling;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace Unity.UIElements.Runtime
{
    internal interface IPanelTransform
    {
        bool ScreenToPanelUV(Vector2 screenPosition, out Vector2 panelPosition);
    }

    /// <summary>
    /// Component to render a UXML file and stylesheets in the game view.
    /// </summary>
    [AddComponentMenu("UIElements/Panel Renderer"), RequireComponent(typeof(PanelScaler)), ExecuteInEditMode]
    public class PanelRenderer : MonoBehaviour, IFileChangedNotify
    {
        class PanelOwner : ScriptableObject
        {
        }

        /// <summary>
        /// The UXML file to render
        /// </summary>
        public VisualTreeAsset uxml;

#if UNITY_EDITOR
        private string m_UxmlAssetPath;
#endif

        /// <summary>
        /// The main style sheet file to give styles to Unity provided elements
        /// </summary>
        public StyleSheet unityStyleSheet;
        
        /// <summary>
        /// The associated stylesheets.
        /// </summary>
        public StyleSheet[] stylesheets;
        
        /// <summary>
        /// The top level element.
        /// </summary>
        public VisualElement visualTree { get; private set; }
        
        /// <summary>
        /// The panel holding the visual tree instantiated from the UXML file.
        /// </summary>
        public IPanel panel { get; protected set; }
        
        /// <summary>
        /// An optional texture onto which the panel should be rendered.
        /// </summary>
        public RenderTexture targetTexture; // world space using render textures
        
        /// <summary>
        /// The transform to apply on the panel.
        /// </summary>
        public Component panelTransform;
        
        /// <summary>
        /// Enables live updates from the UI Builder.
        /// </summary>
        public bool enableLiveUpdates => m_EnableLiveUpdate;
#pragma warning disable 0649
        [SerializeField] private bool m_EnableLiveUpdate;
#pragma warning restore 0649
        
#if UNITY_EDITOR
        private bool m_OldEnableLiveUpdate;
#endif
        
        /// <summary>
        /// Functions called after UXML document has been loaded.
        /// </summary>
        public Func<IEnumerable<Object>> postUxmlReload { get; set; }

        IPanelTransform m_PanelTransform;
        PanelScaler m_PanelScaler;
        PanelOwner m_PanelOwner;
        
        RenderTexture m_TargetTexture;
        Event m_Event = new Event(); // will be used for panel repaint exclusively
        float m_Scale; // panel scaling factor (pixels <-> points)
        Vector2 m_TargetSize;

        CustomSampler m_InitSampler;
        CustomSampler initSampler
        {
            get
            {
                if (m_InitSampler == null)
                    m_InitSampler = CustomSampler.Create("UIElements." + gameObject.name + ".Initialize");

                return m_InitSampler;
            }
        }

        CustomSampler m_UpdateSampler;
        CustomSampler updateSampler
        {
            get
            {
                if (m_UpdateSampler == null)
                    m_UpdateSampler = CustomSampler.Create("UIElements." + gameObject.name + ".Update");

                return m_UpdateSampler;
            }
        }
        
        bool m_ShouldWarnWorldTransformMissing = true;

        /// <summary>
        /// Implementation of OnValidate().
        /// </summary>
        protected void OnValidate()
        {
            Validate();
        }

        /// <summary>
        /// Validate the panel transform.
        /// </summary>
        protected virtual void Validate()
        {
            m_ShouldWarnWorldTransformMissing = true;
            if (panelTransform != null && panelTransform is IPanelTransform)
                m_PanelTransform = panelTransform as IPanelTransform;

#if UNITY_EDITOR
            m_UxmlAssetPath = uxml ? AssetDatabase.GetAssetPath(uxml) : null;
            
            if (m_OldEnableLiveUpdate != m_EnableLiveUpdate)
            {
                if (m_EnableLiveUpdate)
                {
                    RecreateUIFromUxml();
                    StartWatchingFiles();
                }
                else
                {
                    StopWatchingFiles();
                }
                m_OldEnableLiveUpdate = m_EnableLiveUpdate;
            }
#endif
        }

        /// <summary>
        /// Implementation of Reset().
        /// </summary>
        protected void Reset()
        {
#if UNITY_EDITOR
            unityStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.unity.ui.runtime/USS/Default.uss.asset");
#endif
        }

        void Initialize()
        {
            if (panel != null && m_PanelOwner == null)
            {
                Debug.LogWarning("Unexpected state: panel without owner. Panel will leak.");
            }
            
            if (panel == null || m_PanelOwner == null)
            {
                initSampler.Begin();

                if (m_PanelOwner == null)
                    m_PanelOwner = ScriptableObject.CreateInstance<PanelOwner>();

                panel = InternalBridge.CreatePanel(m_PanelOwner);
                var root = panel.visualTree;
                root.name = gameObject.name;

                visualTree = new VisualElement {name = "runtime-panel-container"};
                visualTree.style.overflow = Overflow.Hidden;

                root.Add(visualTree);

                if (unityStyleSheet != null)
                {
                    InternalBridge.MarkAsDefaultStyleSheet(unityStyleSheet);
                    root.styleSheets.Add(unityStyleSheet);
                }

                if (stylesheets != null)
                {
                    foreach (var uss in stylesheets)
                        if (uss != null)
                            root.styleSheets.Add(uss);
                }
                
                initSampler.End();
            }
            
            if (m_PanelScaler == null)
                m_PanelScaler = GetComponent<PanelScaler>();
    
            m_Event.type = EventType.Repaint;
            m_Scale = Single.NaN;
            m_TargetSize = new Vector2(Single.NaN, Single.NaN);
            
            if (m_TargetTexture != targetTexture)
            {
                m_TargetTexture = targetTexture;
                InternalBridge.SetTargetTexture(panel, m_TargetTexture);
            }
            
            Validate();
            
            RecreateUIFromUxml();
        }

        void Cleanup()
        {
            if (m_PanelOwner != null)
            {
                InternalBridge.DisposePanel(m_PanelOwner);
                DestroyImmediate(m_PanelOwner);
            }

            panel = null;
            m_PanelOwner = null;
            m_PanelScaler = null;
        }

        /// <summary>
        /// Implementation of Awake()
        /// </summary>
        protected void Awake()
        {
#if UNITY_EDITOR
            m_OldEnableLiveUpdate = m_EnableLiveUpdate;
#endif
            
            // We try to call Initialize as soon as possible.
            Cleanup();
            Initialize();
        }

        /// <summary>
        /// Implementation of OnEnable()
        /// </summary>
        protected void OnEnable()
        {
            // Sometimes Awake is not called. Ensure we have called Initialize().
            Initialize();
            
            if (m_EnableLiveUpdate)
            {
                StartWatchingFiles();
            }
        }

        private void OnGUI()
        {
            if (!Application.isPlaying)
            {
#if UNITY_EDITOR
                bool redraw = ReloadAssetsOnChange();
            
                DoUpdate();

                if (redraw)
                {
                    ForceGameViewRepaint();
                }
#endif
            }
        }
        
#if UNITY_EDITOR
        private int m_UXMLCachedHash;
        
        int GetRecursiveHashCode(VisualTreeAsset vta)
        {
            int hashCode;
            
#if UNITY_2020_1_OR_NEWER
            unchecked
            {
                hashCode = vta.contentHash;
                
                // When an attribute is edited, we need to recreate the whole UI
                // since UIElements has no way to update existing VisualElements
                // from the asset. 
                hashCode = (hashCode * 397) ^ EditorUtility.GetDirtyCount(vta);

                foreach (var asset in vta.templateDependencies)
                {
                    hashCode = (hashCode * 397) ^ GetRecursiveHashCode(asset);
                }
                
                foreach (var asset in vta.stylesheets)
                {
                    if (asset != null)
                    {
                        hashCode = (hashCode * 397) ^ asset.contentHash;
                        hashCode = (hashCode * 397) ^ EditorUtility.GetDirtyCount(asset);
                    }
                }
            }
#else
            unchecked
            {
                hashCode = vta.GetHashCode();
                
                // When an attribute is edited, we need to recreate the whole UI
                // since UIElements has no way to update existing VisualElements
                // from the asset. 
                hashCode = (hashCode * 397) ^ EditorUtility.GetDirtyCount(vta);

                // We can't detect changes to sub-UXML files (referenced templates) in 2019.3
                // We can't detect external content change to stylesheets in 2019.3
            }
#endif
            
            return hashCode;
        }
#endif
        
        bool ReloadAssetsOnChange()
        {
#if UNITY_EDITOR
            if (enableLiveUpdates)
            {
                if (uxml == null && !string.IsNullOrEmpty(m_UxmlAssetPath))
                {
                    uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(m_UxmlAssetPath);
                }

                if (uxml != null)
                {
                    var hash = GetRecursiveHashCode(uxml);
                    if (hash != m_UXMLCachedHash)
                    {
                        RecreateUIFromUxml();
                        return true;
                    }
                }
            }
#endif
            return false;
        }

        /// <summary>
        /// Implementation of Update()
        /// </summary>
        protected void Update()
        {
            ReloadAssetsOnChange();
            DoUpdate();
        }
        
        protected virtual void DoUpdate()
        {
            updateSampler.Begin();

            var targetSize = targetTexture == null
                ? GetActiveRenderTargetSize()
                : new Vector2(targetTexture.width, targetTexture.height);

            if (targetTexture != m_TargetTexture)
            {
                m_TargetTexture = targetTexture;
                InternalBridge.SetTargetTexture(panel, targetTexture);
            }

            // Temporary: clamp scale to prevent font atlas running out of space
            // won't be needed when using TextCore
            var scale = Mathf.Max(0.1f, m_PanelScaler == null ? 1 : m_PanelScaler.ComputeScalingFactor(targetSize));

            if (m_Scale != scale || m_TargetSize != targetSize)
            {
                InternalBridge.SetScale(panel, scale == 0 ? 0 : 1.0f / scale);
                visualTree.style.left = 0;
                visualTree.style.top = 0;
                visualTree.style.width = targetSize.x * scale;
                visualTree.style.height = targetSize.y * scale;
                m_Scale = scale;
                m_TargetSize = targetSize;
            }

            InternalBridge.UpdatePanel(panel);
            
            updateSampler.End();
        }

        /// <summary>
        /// Implementation of OnRenderObject().
        /// </summary>
        protected void OnRenderObject()
        {
            // render texture based world space rendering
            if (targetTexture != null)
            {
                // when doing world space repaint has to be called explicitly
                InternalBridge.RepaintPanel(panel, m_Event);
            }
        }
        
        /// <summary>
        /// Implementation of OnDisable().
        /// </summary>
        protected void OnDisable()
        {
            StopWatchingFiles();
            // We need to Cleanup() here otherwise panels leak when entering playmode.
            Cleanup();
        }
        
        /// <summary>
        /// Implementation of OnDestroy().
        /// </summary>
        protected void OnDestroy()
        {
            RemoveWatchedFiles();
            Cleanup();
        }
        
        /// <summary>
        /// Force rebuild the UI from UXML (if one is attached).
        /// </summary>
        public void RecreateUIFromUxml()
        {
            if (uxml == null || visualTree == null)
                return;

            visualTree.Clear();
            visualTree.styleSheets.Clear();
            
            uxml.CloneTree(visualTree);
            
#if UNITY_EDITOR
            RemoveWatchedFiles();
            AddWatchedFiles();
            if (enableLiveUpdates)
            {
                m_UXMLCachedHash = GetRecursiveHashCode(uxml);
                StartWatchingFiles();
                ForceGameViewRepaint();
            }
#endif
            
            postUxmlReload?.Invoke();
        }

#if UNITY_EDITOR
        private void ForceGameViewRepaint()
        {
            System.Reflection.Assembly assembly = typeof(EditorWindow).Assembly;
            Type type = assembly.GetType("UnityEditor.GameView");
            EditorWindow gameview = EditorWindow.GetWindow(type);
            gameview.Repaint();
        }
#endif
        
        private static Vector2 GetActiveRenderTargetSize()
        {
            return RenderTexture.active == null
                ? new Vector2(Screen.width, Screen.height)
                : new Vector2(RenderTexture.active.width, RenderTexture.active.height);
        }

        internal bool ScreenToPanel(Vector2 screenPosition, out Vector2 panelPosition)
        {
            // if no target texture is set, screen space overlay is assumed
            if (targetTexture == null)
            {
                panelPosition = screenPosition * m_Scale;
                return true;
            }
            
            // can we delegate to worldtransform?
            if (m_PanelTransform == null)
            {
                if (m_ShouldWarnWorldTransformMissing)
                {
                    m_ShouldWarnWorldTransformMissing = false;
                    Debug.LogError("PanelRenderer needs an IWorldTransform implementation for world-space rendering");
                }
                panelPosition = Vector2.zero;
                return false;
            }

            Vector2 panelUVPosition;
            var hit =  m_PanelTransform.ScreenToPanelUV(screenPosition, out panelUVPosition);

            if (!hit)
            {
                panelPosition = Vector2.zero;
                return false;
            }

            var panelSize = panel.visualTree.layout.size;
            panelPosition = new Vector2(panelUVPosition.x * panelSize.x, panelUVPosition.y * panelSize.y);
            return true;
        }

        public void OnFileChanged(string path)
        {
#if UNITY_EDITOR
            AssetDatabase.Refresh();
            ReloadAssetsOnChange();
#endif
        }
        
        private void AddWatchedFiles()
        {
#if UNITY_EDITOR
            var path = AssetDatabase.GetAssetPath(uxml);
            FileWatcher.Instance().AddFile(this, path);

            foreach (var stylesheet in stylesheets)
            {
                path = AssetDatabase.GetAssetPath(stylesheet);
                FileWatcher.Instance().AddFile(this, path);
            }
            
            AddWatchedFilesForUxml(uxml);
#endif
        }
        
#if UNITY_EDITOR
        private void AddWatchedFilesForUxml(VisualTreeAsset vta)
        {
#if UNITY_2020_1_OR_NEWER
            foreach (var asset in vta.templateDependencies)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                if (!string.IsNullOrEmpty(path))
                {
                    FileWatcher.Instance().AddFile(this, path);
                }
                AddWatchedFilesForUxml(asset);
            }
                
            foreach (var asset in vta.stylesheets)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                if (!string.IsNullOrEmpty(path))
                {
                    FileWatcher.Instance().AddFile(this, path);
                }
            }
#endif
        }
#endif
        
        private void StartWatchingFiles()
        {
#if UNITY_EDITOR
            FileWatcher.Instance().EnableWatcher(this);
#endif
        }

        private void StopWatchingFiles()
        {
#if UNITY_EDITOR
            FileWatcher.Instance().DisableWatcher(this);
#endif
        }

        private void RemoveWatchedFiles()
        {
#if UNITY_EDITOR
            FileWatcher.Instance().RemoveAllFiles(this);
#endif
        }
    }
}