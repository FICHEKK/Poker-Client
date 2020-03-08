using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InputFieldIterator : MonoBehaviour
{
    [SerializeField] private List<TMP_InputField> inputFields;
    [SerializeField] private KeyCode nextInputFieldKey;
    private int selectedFieldIndex;
    
    void Start()
    {
        inputFields.First().Select();

        for (int i = 0; i < inputFields.Count; i++)
        {
            var index = i;
            inputFields[i].onSelect.AddListener(arg => selectedFieldIndex = index);
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(nextInputFieldKey))
        {
            selectedFieldIndex++;

            if (selectedFieldIndex == inputFields.Count)
            {
                selectedFieldIndex = 0;
            }
            
            inputFields[selectedFieldIndex].Select();
        }
    }
}
