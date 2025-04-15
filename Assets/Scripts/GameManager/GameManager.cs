using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public abstract class GameManager : MonoBehaviour
{

    //Máu
    public float Health;            //Máu
    public float MaxHealth;         //Máu tối đa

    //Stamina
    public float Stamina;           //Năng lượng
    public float MaxStamina;        //Năng lượng tối đa

    //Mana
    public float Mana;              //Mana
    public float MaxMana;           //Mana tối đa

    //Giáp
    public float Armor;             //Giáp chung
    public float ArmorPoison;       //Giáp chống độc
    public float ArmorFire;         //Giáp chống cháy
    public float ArmorMagic;        //Giáp chống phép thuật

    //Kiểm tra
    public bool Magic;              //Kiểm tra có dính phép thuật hay không
    public bool Poison;             //Kiểm tra có dính độc hay không
    public bool Fire;               //Kiểm tra có dính cháy hay không

    //Thời gian
    public float TimeMagic;         //Thời gian dính phép thuật
    public float TimePoison;        //Thời gian dính độc
    public float TimeFire;          //Thời gian dính cháy

    //Dame
    public float Damage;            //Dame chung
    public float DamageMagic;       //Dame phép thuật
    public float DamagePhysical;    //Dame vật lý

    //Critical
    public float Critical;          //Chí mạng chung
    public float CriticalMagic;     //Chí mạng phép thuật
    public float CriticalPhysical;    //Chí mạng vật lý



}
