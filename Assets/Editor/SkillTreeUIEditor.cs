#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI.Extensions;

[CustomEditor(typeof(SkillTreeUI))]
public class SkillTreeUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        DrawDefaultInspector();

        SkillTreeUI skillTreeUI = (SkillTreeUI)target;

        if (GUILayout.Button("Preview Skill Tree"))
        {
            PreviewSkillTree(skillTreeUI);
        }
    }

    private void PreviewSkillTree(SkillTreeUI skillTreeUI)
    {
        SkillTreeNode[] nodes = skillTreeUI.GetComponentsInChildren<SkillTreeNode>();

        if (nodes.Length == 0)
        {
            Debug.LogWarning("No SkillTreeNode children found under SkillTreeUI.");
            return;
        }

        Undo.RecordObjects(GetTransforms(nodes), "Update Skill Tree Nodes");

        foreach (var node in nodes)
        {
            Vector2 newPos = new(node.GridPos.x * skillTreeUI.GridSpacing, node.GridPos.y * skillTreeUI.GridSpacing);
            node.GetComponent<RectTransform>().localPosition = newPos;
            EditorUtility.SetDirty(node);
        }

        skillTreeUI.InitializeLineRenderers(nodes);

        foreach (var node in nodes)
        {
            for (int i = 0; i < node.transform.childCount; i++)
            {
                EditorUtility.SetDirty(node.transform.GetChild(i).GetComponent<UILineConnector>());
            }
        }

        EditorSceneManager.MarkSceneDirty(skillTreeUI.gameObject.scene);

        Debug.Log($"Updated {nodes.Length} node(s).");
    }

    private Transform[] GetTransforms(SkillTreeNode[] nodes)
    {
        Transform[] transforms = new Transform[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            transforms[i] = nodes[i].transform;
        }
        return transforms;
    }
}

#endif