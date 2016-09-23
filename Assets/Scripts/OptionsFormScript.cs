using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class OptionsFormScript : MonoBehaviour {

    #region Properties

    public int Mines {
        get { return int.Parse(Get("Text", "Mines").text); }
    }

    public string MinesPlaceholder {
        set { Get("Placeholder", "Mines").text = value; }
    }

    public int FieldWidth {
        get { return int.Parse(Get("Text", "FieldWidth").text); }
    }

    public string FieldWidthPlaceholder {
        set { Get("Placeholder", "FieldWidth").text = value; }
    }

    public int FieldHeight {
        get { return int.Parse(Get("Text", "FieldHeight").text); }
    }

    public string FieldHeightPlaceholder {
        set { Get("Placeholder", "FieldHeight").text = value; }
    }
    #endregion

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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

    private string GetParentName(GameObject obj) {
        return obj.transform.parent.transform.parent.name;
    }
}
