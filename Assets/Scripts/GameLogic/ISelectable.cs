using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelectable<T>
{
    //T getSelectedValue();

    void populateItemData(T item);

    Sprite getSprite();

}
