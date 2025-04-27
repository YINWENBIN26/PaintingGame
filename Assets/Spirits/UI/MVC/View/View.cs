/****************************************************
	文件：View.cs
	作者：小小泽
	邮箱：1245615197@qq.com
	日期：#CreateTime#   	
	功能：Nothing
*****************************************************/
using UnityEngine;

public class View : MonoBehaviour
{
	private static View _view;
	public static View GetView
    {
        get
        {
			if (_view == null)
				_view = new View();
			return _view;
        }
    }

	public void Init()
    {

    }

	public void UpdateInfo()
    {

    }

}
