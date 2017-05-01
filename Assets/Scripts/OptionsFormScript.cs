using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class OptionsFormScript : MonoBehaviour {

    #region Properties
    /// <summary>
    /// Gets the value of the 'Mines' field
    /// </summary>
    public int Mines {
        get { return int.Parse(Get("Text", "Mines").text); }
    }

    /// <summary>
    /// Gets the value of the 'Width' field
    /// </summary>
    public int FieldWidth {
        get { return int.Parse(Get("Text", "FieldWidth").text); }
    }

    /// <summary>
    /// Gets the value of the 'Height' field
    /// </summary>
    public int FieldHeight {
        get { return int.Parse(Get("Text", "FieldHeight").text); }
    }
    #endregion

    /// <summary>
    /// Gets the field with name <paramref name="name"/> and parent <paramref name="parentName"/>
    /// </summary>
    /// <param name="name">Name of the <see cref="Text"/> object</param>
    /// <param name="parentName">Name of the parent of the object</param>
    /// <returns></returns>
    private Text Get(string name, string parentName) {
        var texts =
            this.GetComponentsInChildren<Text>()
                .Where(text => text.name == name && text.transform.parent.transform.parent.name == parentName);
        if (texts.Count() == 1)
            return texts.First();

        string temp = " text field found named " + name + " of parent " + parentName;
        if (!texts.Any())
            throw new MissingComponentException("No" + temp);
        throw new MissingComponentException("Multiple" + temp);
    }
}
