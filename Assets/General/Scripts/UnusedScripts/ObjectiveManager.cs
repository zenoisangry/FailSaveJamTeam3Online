using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Objective
{
    public string id;
    [TextArea] public string description;
    public bool completed;
}

public class ObjectiveManager : MonoBehaviour
{
    public static ObjectiveManager Instance;
    public TMP_Text objectiveText;
    public List<Objective> objectives = new List<Objective>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        UpdateObjectiveUI();
    }

    public void SetObjective(string id)
    {
        foreach (var obj in objectives)
        {
            obj.completed = obj.id != id;
        }

        var current = objectives.Find(o => o.id == id);
        if (current != null)
        {
            current.completed = false;
            UpdateObjectiveUI(current.description);
        }
    }

    public void CompleteObjective(string id)
    {
        var obj = objectives.Find(o => o.id == id);
        if (obj != null) obj.completed = true;
        UpdateObjectiveUI();
    }

    public void UpdateObjectiveUI(string text = "")
    {
        if (objectiveText == null) return;
        var current = objectives.Find(o => !o.completed);
        if (current != null) text = current.description;

        objectiveText.text = text;
    }
}