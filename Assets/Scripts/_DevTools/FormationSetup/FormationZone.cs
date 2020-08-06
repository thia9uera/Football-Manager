using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FormationZone : MonoBehaviour, IPointerClickHandler
{
    public Zone Zone;

    private TextMeshProUGUI label;

    private bool isSelected;
    private Image img;

    public bool isActive;

    public void Awake()
    {
        img = GetComponent<Image>();
        label = GetComponentInChildren<TextMeshProUGUI>();
        label.text = Zone.ToString();
    }

    public void LeftClickHandler()
    {
        if (!isActive) return;
        FormationSetup.Instance.SelectZone(this);
    }

    public void Activate(bool _isActive)
    {
        isActive = _isActive;
        if (_isActive)
        {
            img.color = Color.gray;
            label.color = Color.white;
        }
        else Deactivate();
    }

    public void Deactivate()
    {
        isActive = false;
        img.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        label.color = Color.gray;
    }

    public void Select()
    {
        img.color = Color.green;
    }

    public void Deselect()
    {
        img.color = Color.gray;
        if (!isActive) Deactivate();
    }

    public void Connect()
    {
        if (!isActive) return;
        FormationSetup.Instance.Connect(this);
    }

    private void MiddleClickHandler()
    {
        FormationSetup.Instance.ActivateZone(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            LeftClickHandler();
        else if (eventData.button == PointerEventData.InputButton.Right)
            Connect();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            MiddleClickHandler();
    }
}