using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Localization drop down example.
/// </summary>
public class LocalizationHelper : MonoBehaviour
{
    [Tooltip("The drop down box to populate with flags")]
    public TMP_Dropdown dropDown;

    [Tooltip("The flags of the languages that our game supports")]
    public Sprite[] flags;

    /// <summary>
    /// Start this instance!
    /// </summary>
    void Start()
    {
        // Clear any existing options, just in case
        dropDown.ClearOptions();

        // Create the option list
        List<TMP_Dropdown.OptionData> flagItems = new List<TMP_Dropdown.OptionData>();

        // Loop through each sprite
        foreach (var flag in flags)
        {
            // Try and find the '.' in the sprite's name. This is used as a delimiter
            // between the country code and the name of the language
            string flagName = flag.name;
            int dot = flag.name.IndexOf('.');
            if (dot >= 0)
            {
                // Found? Then set the flag name to the characters to the right of the dot
                flagName = flagName.Substring(dot + 1);
            }

            // Add the option to the list
            var flagOption = new TMP_Dropdown.OptionData(flagName, flag);
            flagItems.Add(flagOption);
        }

        // Add the options to the drop down box
        dropDown.AddOptions(flagItems);
    }
}
