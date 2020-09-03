using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(MeshManager))]
public class MeshGenUI : Editor
{
    private MeshManager _meshManager;
    private VisualElement _visualElement;
    private VisualTreeAsset _visualTreeAsset;
    private StyleSheet _uss;

    private void OnEnable()
    {
        _meshManager = target as MeshManager;
        _visualElement = new VisualElement();
        _visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/MeshGeneration.uxml");
        _uss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/MeshGeneration.uss");
        _visualElement.styleSheets.Add(_uss);
    }

    public override VisualElement CreateInspectorGUI()
    {
        var root = _visualElement;
        root.Clear();
        _visualTreeAsset.CloneTree(root);

        var button = root.Q<Button>("Generate");
        button.clickable.clicked += () =>
        {
            _meshManager.Generate();
        };
        
        return root;
    }
}
