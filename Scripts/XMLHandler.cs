using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using UnityEngine.Networking;
using System.IO;

public class XMLHandler : MonoBehaviour
{
    XmlDocument xmlDocReader = new();
    //string path = Path.Combine(Application.streamingAssetsPath, "AttackDefencePairings.xml"); - old method using StreamingAssets
    string path_attack_defence = "https://raw.githubusercontent.com/Alex-Lehm/DissertationFileHosting/main/AttackDefencePairings.xml";
    string path_defence_desc = "https://raw.githubusercontent.com/Alex-Lehm/DissertationFileHosting/main/DefenceDescriptions.xml";
    string result_attack_defence = "";
    string result_defence_desc = "";

    IEnumerator Start()
    {
        Debug.Log("Running on: " + Application.platform);
        Debug.Log("Beginning UnityWebRequest 1...");
        using (UnityWebRequest uwr = UnityWebRequest.Get(path_attack_defence)) // Creating UWR
        {
            yield return uwr.SendWebRequest(); // Sending request
            result_attack_defence = uwr.downloadHandler.text; // Response returned as string
        }
        Debug.Log("Request 1 complete");


        Debug.Log("Beginning UnityWebRequest 2...");
        using (UnityWebRequest uwr = UnityWebRequest.Get(path_defence_desc))
        {
            yield return uwr.SendWebRequest();
            result_defence_desc = uwr.downloadHandler.text;
        }
        Debug.Log("Request 2 complete");

        // Begin game setup now that async web requests are complete
        GetComponent<HelpWindow>().PostXMLHelpSetup();
        GetComponent<GameFlowCore>().PostXMLLoadStart();
    }

    public Dictionary<string, string> LoadDefenceDescriptions() {
        xmlDocReader.LoadXml(result_defence_desc);

        XmlNodeList defence_descs = xmlDocReader.SelectNodes("//defence-desc"); // the base node (not root)

        Dictionary<string, string> defenceDescList = new();

        // Iterating through each base node
        foreach (XmlNode defence_desc in defence_descs)
        {
            string defence = defence_desc.SelectSingleNode("defence").InnerText;
            defenceDescList[defence] = defence_desc.SelectSingleNode("desc").InnerText;
        }

        return defenceDescList;
    }

    public Dictionary<string, List<string>> LoadAttackDefenceData() {
        xmlDocReader.LoadXml(result_attack_defence);

        XmlNodeList pairings = xmlDocReader.SelectNodes("//pair"); // the base node (not root)

        Dictionary<string, List<string>> attackDefenceMapping = new();

        // Iterating through each base node
        foreach (XmlNode pairing in pairings)
        {
            string attack = pairing.SelectSingleNode("attack").InnerText;
            attackDefenceMapping[attack] = new List<string>();

            // Iterating through each viable defence per attack
            XmlNodeList defences = pairing.SelectNodes("defence");
            foreach (XmlNode defence in defences)
            {
                attackDefenceMapping[attack].Add(defence.InnerText);
            }
        }

        return attackDefenceMapping;
    }
}
