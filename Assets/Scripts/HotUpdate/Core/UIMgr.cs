using FairyGUI;
using System.Collections.Generic;
using System.Linq;

public class UIMgr : Singleton<UIMgr>
{

    #region UIView-->GComponent
    private Dictionary<string, GComponent> mShowGCompDic = new Dictionary<string, GComponent>();
    public T OpenUIViewCom<T>(string pkgName) where T : GComponent, new()
    {
        string viewName = (typeof(T).Name);
        var gCom = UIPackage.CreateObject(pkgName, viewName).asCom;
        gCom.MakeFullScreen();
        GRoot.inst.AddChild(gCom);
        gCom.AddRelation(GRoot.inst, RelationType.Size);
        gCom.fairyBatching = true;
        gCom.OnInit();//ҳ�淽����  һ��onClick,onClickItem,itemRenderer...�ڴ˴�����

        mShowGCompDic[viewName] = gCom;
        return gCom as T;
    }

    public void CloseUIViewCom<T>() where T : GComponent
    {
        var viewName = (typeof(T).Name);
        if (mShowGCompDic.TryGetValue(viewName, out var gCom))
        {
            gCom.Dispose();
            mShowGCompDic.Remove(viewName);

            CheckRemovePackage(gCom.packageItem.owner.name);//�ر�ҳ��� ��� �Ƿ�Ҫ�����ͷŰ� 
        }
    }

    public GComponent GetUIViewCom<T>() where T : GComponent
    {
        var viewName = (typeof(T).Name);
        if (mShowGCompDic.TryGetValue(viewName, out var viewCls))
        {
            return viewCls;
        }
        return null;
    }
    /// <summary>     ���� UIView������     </summary>
    /// <param name="isActive"> true��ʾ,,,false����</param>
    public GComponent SetActiveUIViewCom<T>(bool isActive) where T : GComponent
    {
        var viewName = (typeof(T).Name);
        if (mShowGCompDic.TryGetValue(viewName, out var viewCls))
        {
            viewCls.visible = isActive;
            return viewCls;
        }
        return null;
    }
    #endregion

    #region FGUIWindow--Window

    private Dictionary<string, FairyGUI.Window> mShowWinDic = new Dictionary<string, FairyGUI.Window>();
    public T OpenWindow<T>() where T : FairyGUI.Window, new()
    {
        var winName = (typeof(T).Name);
        T t = new T();
        t.Show();
        mShowWinDic[winName] = t;
        return t as T;
    }

    public void CloseWindow<T>() where T : FairyGUI.Window
    {
        var winName = (typeof(T).Name);
        if (mShowWinDic.TryGetValue(winName, out var winCls))
        {
            winCls.Dispose();

            mShowWinDic.Remove(winName);
            CheckRemovePackage(winCls.contentPane.packageItem.owner.name);
        }
    }

    public void CloseWindowExpand(Window gWindow)
    {
        for (int i = 0; i < mShowWinDic.Count; i++)
        {
            var item = mShowWinDic.ElementAt(i);
            if (item.Value == gWindow)
            {
                gWindow.Dispose();
                mShowWinDic.Remove(item.Key);
                CheckRemovePackage(gWindow.contentPane.packageItem.owner.name);//�ر�win
                return;
            }
        }
    }

    /// <summary>     ���� UIView������     </summary>
    /// <param name="isActive"> true��ʾ,,,false����</param>
    public Window SetActiveUIWindow<T>(bool isActive) where T : FairyGUI.Window
    {
        var winName = (typeof(T).Name);
        if (mShowWinDic.TryGetValue(winName, out var winCls))
        {
            if (isActive) winCls.Show();
            else winCls.Hide();

            return winCls;
        }
        return null;
    }


    #endregion
    private void CheckRemovePackage(string pkgName)
    {
        var tIsNeed = true;
        foreach (var item in mShowGCompDic)
        {
            if (item.Value.packageItem.owner.name.Equals(pkgName))
            {
                tIsNeed = false;//����ҳ������ʹ�� �����
                return;
            }
        }

        if (tIsNeed)
        {
            foreach (var item in mShowWinDic)
            {
                if (item.Value.contentPane.packageItem.owner.name.Equals(pkgName))
                {
                    tIsNeed = false;//����Window����ʹ�� �����
                    return;
                }
            }
        }

        if (tIsNeed)
        {
            FGUILoader.Instance.RemoveUIPackage(pkgName);
        }
    }
}
