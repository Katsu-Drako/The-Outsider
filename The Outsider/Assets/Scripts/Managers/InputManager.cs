using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : PersistentManager
{
    protected const string _mouseX = "Mouse X", _mouseY = "Mouse Y", _mouseScrollWheel = "Mouse ScrollWheel";
    public float MouseSensitivity = 5f, ZoomSensitivity = 5f, MinZoom, MaxZoom = 10f;
    [Header("Default snappiness 0.25, default gravity 0.5")]
    public List<Axis> Axi = DefaultAxi();
    public List<MouseButtonAxis> MouseButtonAxi = DefaultMouseButtonAxi();
    public List<Command> Commands = DefaultCommands();
    [SerializeField, HideInInspector]
    protected float _zoom;
    [SerializeField, HideInInspector]
    protected Vector2 _orbitXY;
    public float Zoom {
        get { return _zoom; }
    }
    public Vector2 OrbitXY {
        get { return _orbitXY; }
    }
    public static List<Axis> DefaultAxi() {
        List<Axis> axis = new List<Axis>();
        axis.Add(new Axis(EAxis.Horizontal, KeyCode.D, KeyCode.A, KeyCode.RightArrow, KeyCode.LeftArrow));
        axis.Add(new Axis(EAxis.Vertical, KeyCode.W, KeyCode.S, KeyCode.UpArrow, KeyCode.DownArrow));
        return axis;
    }
    public static List<MouseButtonAxis> DefaultMouseButtonAxi() {
        List<MouseButtonAxis> axis = new List<MouseButtonAxis>();
        axis.Add(new MouseButtonAxis(0));
        axis.Add(new MouseButtonAxis(1));
        return axis;
    }
    public static List<Command> DefaultCommands() {
        List<Command> commands = new List<Command>();
        commands.Add(new Command(ECommand.Jump, KeyCode.Space));
        commands.Add(new Command(ECommand.Interact, KeyCode.F));
        return commands;
    }
    public float GetAxis(EAxis eAxis) {
        Axis axis = Axi.Find(x => x.EAxis == eAxis);
        if (axis != null) {
            return axis.Value;
        }
        return 0;
    }
    public float GetAxisDelta(EAxis eAxis) {
        Axis axis = Axi.Find(x => x.EAxis == eAxis);
        if (axis != null) {
            return axis.Delta;
        }
        return 0;
    }
    public float GetMouseButtonAxis(int button) {
        MouseButtonAxis axis = MouseButtonAxi.Find(x => x.Button == button);
        if (axis != null) {
            return axis.Value;
        }
        return 0;
    }
    public float GetMouseButtonAxisDelta(int button) {
        MouseButtonAxis axis = MouseButtonAxi.Find(x => x.Button == button);
        if (axis != null) {
            return axis.Delta;
        }
        return 0;
    }
    public bool GetButtonDown(ECommand eCommand) {
        Command command = Commands.Find(x => x.ECommand == eCommand);
        if (command != null) {
            return Input.GetKeyDown(command.KeyCode);
        }
        return false;
    }
    public bool GetButton(ECommand eCommand) {
        Command command = Commands.Find(x => x.ECommand == eCommand);
        if (command != null) {
            return Input.GetKey(command.KeyCode);
        }
        return false;
    }
    public bool GetButtonUp(ECommand eCommand) {
        Command command = Commands.Find(x => x.ECommand == eCommand);
        if (command != null) {
            return Input.GetKeyUp(command.KeyCode);
        }
        return false;
    }
    protected virtual void Update() {
        UpdateOrbit();
        UpdateZoom();
        UpdateAxi();
    }
    protected void UpdateOrbit() {
        _orbitXY += new Vector2(Input.GetAxis(_mouseX), Input.GetAxis(_mouseY)) * MouseSensitivity;
        _orbitXY.y = Mathf.Clamp(_orbitXY.y, -90f, 90f);
    }
    protected void UpdateZoom() {
        _zoom += -Input.GetAxis(_mouseScrollWheel) * ZoomSensitivity;
        _zoom = Mathf.Clamp(_zoom, MinZoom, MaxZoom);
    }
    protected void UpdateAxi() {
        foreach (Axis axis in Axi) {
            axis.Update();
        }
        foreach (MouseButtonAxis axis in MouseButtonAxi) {
            axis.Update();
        }
    }
}
[Serializable]
public class Axis {
    [HideInInspector]
    public string Name;
    public EAxis EAxis;
    public KeyCode Positive, Negative, AltPositive, AltNegative;
    [Range(0, 1f)]
    public float Snappiness = .25f, Gravity = .5f;
    [SerializeField, HideInInspector]
    protected bool _bPositive, _bNegative;
    [SerializeField, HideInInspector]
    protected float _value, _lastValue, _delta;
    public float Value {
        get { return _value; }
    }
    public float Delta {
        get { return _delta; }
    }
    public Axis(EAxis eAxis, KeyCode positive, KeyCode negative, KeyCode altPositive, KeyCode altNegative) {
        Name = eAxis.ToString();
        EAxis = eAxis;
        Positive = positive;
        Negative = negative;
        AltPositive = altPositive;
        AltNegative = altNegative;
    }
    public void Update() {
        _lastValue = _value;
        _bPositive = Input.GetKey(Positive) || Input.GetKey(AltPositive);
        _bNegative = Input.GetKey(Negative) || Input.GetKey(AltNegative);
        if (!(_bPositive && _bNegative) && (_bPositive || _bNegative)) {
            _value = Mathf.Lerp(_value, _bPositive ? 1f : -1f, Snappiness);
        }
        else {
            _value = Mathf.Lerp(_value, 0, Gravity);
        }
        _delta = _value - _lastValue;
    }
}
public enum EAxis {
    Horizontal,
    Vertical,
    RightHand,
    LeftHand,
}
[Serializable]
public class MouseButtonAxis {
    [HideInInspector]
    public string Name;
    public int Button;
    [Range(0, 1f)]
    public float Snappiness = .25f, Gravity = .5f;
    [SerializeField, HideInInspector]
    protected float _value, _lastValue, _delta;
    public float Value {
        get { return _value; }
    }
    public float Delta {
        get { return _delta; }
    }
    public MouseButtonAxis(int button) {
        Name = button.ToString();
        Button = button;
    }
    public void Update() {
        _lastValue = _value;
        _value = Input.GetMouseButton(Button) ? Mathf.Lerp(_value, 1f, Snappiness) : Mathf.Lerp(_value, 0, Gravity);
        _delta = _value - _lastValue;
    }
}
public enum ECommand {
    Jump,
    Interact
}
[Serializable]
public class Command {
    [HideInInspector]
    public string Name;
    public ECommand ECommand;
    public KeyCode KeyCode;
    public Command(ECommand eCommand, KeyCode keyCode) {
        Name = eCommand.ToString();
        ECommand = eCommand;
        KeyCode = keyCode;
    }
}