#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System;
using UnityEditor.IMGUI.Controls;

namespace UniRx.Async.Editor
{
    public class UniTaskTrackerWindow : EditorWindow
    {
        static UniTaskTrackerWindow window;

        [MenuItem("Window/UniRx/UniTask Tracker")]
        public static void OpenWindow()
        {
            if (window != null)
            {
                window.Close();
            }

            // will called OnEnable(singleton instance will be set).
            GetWindow<UniTaskTrackerWindow>("UniTask Tracker").Show();
        }

        static readonly GUILayoutOption[] EmptyLayoutOption = new GUILayoutOption[0];

        object splitterState;

        void OnEnable()
        {
            window = this; // set singleton.
            splitterState = SplitterGUILayout.CreateSplitterState(new float[] { 75f, 25f }, new int[] { 32, 32 }, null);
        }

        void OnGUI()
        {
            // Head
            RenderHeadPanel();

            // Splittable
            SplitterGUILayout.BeginVerticalSplit(this.splitterState, EmptyLayoutOption);
            {
                // Column Tabble
                RenderTable();

                // StackTrace details
                RenderDetailsPanel();

            }
            SplitterGUILayout.EndVerticalSplit();
        }

        #region HeadPanel

        public static bool EnableTracking { get; private set; }
        public static bool EnableStackTrace { get; private set; }
        static readonly GUIContent EnableTrackingHeadContent = EditorGUIUtility.TrTextContent("Enable Tracking", "Start to track async/await UniTask(Require Replay). Performance impact: low", (Texture)null);
        static readonly GUIContent EnableStackTraceHeadContent = EditorGUIUtility.TrTextContent("Enable StackTrace", "Capture StackTrace when task is started(Require Replay). Performance impact: high", (Texture)null);

        // [Enable Tracking] | [Enable StackTrace]
        void RenderHeadPanel()
        {
            EditorGUILayout.BeginVertical(EmptyLayoutOption);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, EmptyLayoutOption);

            if (GUILayout.Toggle(EnableTracking, EnableTrackingHeadContent, EditorStyles.toolbarButton, EmptyLayoutOption) != EnableTracking)
            {
                EnableTracking = !EnableTracking;
            }

            if (GUILayout.Toggle(EnableStackTrace, EnableStackTraceHeadContent, EditorStyles.toolbarButton, EmptyLayoutOption) != EnableStackTrace)
            {
                EnableStackTrace = !EnableStackTrace;
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region TableColumn

        Vector2 tableScroll;
        GUIStyle tableListStyle;

        void RenderTable()
        {
            if (tableListStyle == null)
            {
                tableListStyle = new GUIStyle("CN Box");
                tableListStyle.margin.top = 0;
                tableListStyle.padding.left = 3;
            }

            EditorGUILayout.BeginVertical(tableListStyle, EmptyLayoutOption);

            this.tableScroll = EditorGUILayout.BeginScrollView(this.tableScroll, new GUILayoutOption[]
            {
                GUILayout.ExpandWidth(true),
                GUILayout.MaxWidth(2000f)
            });
            var controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[]
            {
                GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true)
            });

            // int controlID = GUIUtility.GetControlID(FocusType.Keyboard);
            // this.m_TestListTree.OnGUI(controlRect, controlID);

            var state = new TreeViewState();
            var treeView = new ExampleTreeView(state);
            treeView.OnGUI(controlRect);

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region Details

        static GUIStyle detailsStyle;
        Vector2 detailsScroll;

        void RenderDetailsPanel()
        {
            if (detailsStyle == null)
            {
                detailsStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
                detailsStyle.wordWrap = false;
                detailsStyle.stretchHeight = true;
                detailsStyle.margin.right = 15;
            }

            detailsScroll = EditorGUILayout.BeginScrollView(this.detailsScroll, EmptyLayoutOption);
            var vector = detailsStyle.CalcSize(new GUIContent("hogehogehugahuga"));
            EditorGUILayout.SelectableLabel("hogehogehugahuga", detailsStyle, new GUILayoutOption[]
            {
                GUILayout.ExpandHeight(true),
                GUILayout.ExpandWidth(true),
                GUILayout.MinWidth(vector.x),
                GUILayout.MinHeight(vector.y)
            });
            EditorGUILayout.EndScrollView();
        }

        #endregion
    }


    public class ExampleTreeView : TreeView
    {
        public ExampleTreeView(TreeViewState state)
            : this(state, new MultiColumnHeader(new MultiColumnHeaderState(new[]
            {
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent("TaskType"), autoResize = true, canSort = true },
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Elapsed"), autoResize = true, canSort = true },
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Status"), autoResize = true, canSort = true },
                new MultiColumnHeaderState.Column() { headerContent = new GUIContent("Position"), autoResize = true, canSort = true },
            })))
        {
        }

        ExampleTreeView(TreeViewState state, MultiColumnHeader header)
            : base(state, header)
        {
            rowHeight = 20;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            header.sortingChanged += Header_sortingChanged;

            header.ResizeToFit();
            Reload();
        }

        private void Header_sortingChanged(MultiColumnHeader multiColumnHeader)
        {
            // throw new NotImplementedException();
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem(id: 1, depth: -1, displayName: "Root");

            root.children = new List<TreeViewItem>
            {
                new TreeViewItem(id: 2, depth: 0, displayName: "Abcde"),
                new TreeViewItem(id: 3, depth: 1, displayName: "Fghij"), // depth 1?
            };

            return root;
        }
    }









}

#endif