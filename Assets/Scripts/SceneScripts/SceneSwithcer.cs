using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

//                              ГАЙД ПО ПЕРЕКЛЮЧЕНИЮ МЕЖДУ СЦЕН (Код ниже)
// 
// 1. КАК ЗАПУСТИТЬ
//     Вызовите метод SwitchToShop(индекс_сцены) //0 - Магазин№1, 1 - Магазин №2

public class SceneSwithcer : MonoBehaviour
{
    public void SwitchToShop(int id)
    {
        if(SceneManager.GetActiveScene().buildIndex != id)
        {
            SceneManager.LoadScene(id);
            SaveSystem.Instance.MarkDirty();
        }
    }
}