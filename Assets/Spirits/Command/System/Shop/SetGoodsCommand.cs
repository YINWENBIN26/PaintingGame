using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;


namespace FrameWork.Command
{
    public struct SetGoodsCommand : ICommand
    {
        public List<int> GoodsTraget;
        private NPCManager nPCController;
        public  void Execute()
        {
            nPCController = GameStateManager._Instance.nPCManager;
            for (int i = 0; i < GoodsTraget.Count; i++) {
                nPCController.targetTranList[GoodsTraget[i]].IsTarget += 1;
             }
        }

    }
}