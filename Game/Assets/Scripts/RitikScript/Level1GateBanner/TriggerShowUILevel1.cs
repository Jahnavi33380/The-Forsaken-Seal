//using UnityEngine;

//public class TriggerShowUILevel1 : MonoBehaviour
//{
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
using UnityEngine;

public class TriggerShowUILevel1 : MonoBehaviour
{
    public GameObject bossHealthBar;

    private void Start()
    {
        bossHealthBar.SetActive(false);  
    }

    public void ShowBar()
    {
        bossHealthBar.SetActive(true);   
    }

    public void HideBar()
    {
        bossHealthBar.SetActive(false); 
    }
}