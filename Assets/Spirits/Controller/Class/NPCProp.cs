using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCprop 
{
    public string ResName;
    public int coin;
    public int MAXafford;
    public int Dealafford;
    public int Minafford;
    public NPCprop(string ResName,int coin,int Minafford, int Dealafford, int Maxafford)
    {
        this.ResName = ResName;
        this.coin = coin;
        this.MAXafford = Maxafford;
        this.Dealafford = Dealafford;
        this.Minafford = Minafford;
    }
}
public enum NPCState
{
    idle=1,
    wait=2,
    deal=3,
    leave=4,
}
