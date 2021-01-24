// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input/InputController.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputController : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputController()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputController"",
    ""maps"": [
        {
            ""name"": ""Basic"",
            ""id"": ""d0a5939c-d032-4d95-b867-483ec16f086f"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""966c80cf-8118-4627-82d3-091375ae8839"",
                    ""expectedControlType"": ""Stick"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AimMouse"",
                    ""type"": ""Value"",
                    ""id"": ""47fa4f4d-3182-41dc-b7ed-d11d821927f0"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""AimController"",
                    ""type"": ""Value"",
                    ""id"": ""6e73f515-1547-47d4-884a-8d4921763d16"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""6b2b7dbd-36bd-4d97-8a58-1617c212ab9c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwapWeapon1"",
                    ""type"": ""Value"",
                    ""id"": ""53e4b0bd-d357-405f-ab28-2de71160d8f8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwapWeapon2"",
                    ""type"": ""Value"",
                    ""id"": ""591008d7-814c-4d7b-b655-91f00c4299f3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwapWeapon3"",
                    ""type"": ""Value"",
                    ""id"": ""05d8c2b4-0dfc-4779-9e9b-099e31ba671e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwapWeapon4"",
                    ""type"": ""Value"",
                    ""id"": ""684ccd73-2a8b-4bc3-a2d3-a5380710e8ed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SwapWeapon5"",
                    ""type"": ""Value"",
                    ""id"": ""c95521d6-5d4c-4345-bb52-43c8398728a2"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""1a9ec0d7-c77c-4428-9758-adc895dadcfb"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ce3b6ef0-cd34-453f-b637-7bd36e55d50f"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""be636688-4c35-4b1d-b295-b579eef59353"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""cd3c0262-d4d8-4105-8cf1-64a23973f476"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8f62dc3c-9e23-4592-8bfc-12466aa86cdc"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""4b143e15-78ec-44b2-b31b-75a6cefc20c9"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f60d203e-730e-478d-b2b0-2053b7fb6610"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ec756a3e-e5cf-47c3-9ea1-f26f6963bec3"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapWeapon1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""bb2dd111-bbd2-4587-ae60-94fde1359c0a"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapWeapon2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""be74e30c-c954-4576-b3fc-50450fc463b2"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapWeapon3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""35eab877-e914-4a9a-a105-594243496a4b"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapWeapon4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4570feca-53b1-4633-a1b0-20700cb3f4aa"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwapWeapon5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""84d594d3-3815-4d24-8b5d-3c096c8cf8d8"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AimMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""57730711-120b-4044-a72c-fd8d26800dba"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AimMouse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""37f5cf89-017b-4cf3-b27a-50ab1d573896"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AimController"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Basic
        m_Basic = asset.FindActionMap("Basic", throwIfNotFound: true);
        m_Basic_Movement = m_Basic.FindAction("Movement", throwIfNotFound: true);
        m_Basic_AimMouse = m_Basic.FindAction("AimMouse", throwIfNotFound: true);
        m_Basic_AimController = m_Basic.FindAction("AimController", throwIfNotFound: true);
        m_Basic_Shoot = m_Basic.FindAction("Shoot", throwIfNotFound: true);
        m_Basic_SwapWeapon1 = m_Basic.FindAction("SwapWeapon1", throwIfNotFound: true);
        m_Basic_SwapWeapon2 = m_Basic.FindAction("SwapWeapon2", throwIfNotFound: true);
        m_Basic_SwapWeapon3 = m_Basic.FindAction("SwapWeapon3", throwIfNotFound: true);
        m_Basic_SwapWeapon4 = m_Basic.FindAction("SwapWeapon4", throwIfNotFound: true);
        m_Basic_SwapWeapon5 = m_Basic.FindAction("SwapWeapon5", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Basic
    private readonly InputActionMap m_Basic;
    private IBasicActions m_BasicActionsCallbackInterface;
    private readonly InputAction m_Basic_Movement;
    private readonly InputAction m_Basic_AimMouse;
    private readonly InputAction m_Basic_AimController;
    private readonly InputAction m_Basic_Shoot;
    private readonly InputAction m_Basic_SwapWeapon1;
    private readonly InputAction m_Basic_SwapWeapon2;
    private readonly InputAction m_Basic_SwapWeapon3;
    private readonly InputAction m_Basic_SwapWeapon4;
    private readonly InputAction m_Basic_SwapWeapon5;
    public struct BasicActions
    {
        private @InputController m_Wrapper;
        public BasicActions(@InputController wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Basic_Movement;
        public InputAction @AimMouse => m_Wrapper.m_Basic_AimMouse;
        public InputAction @AimController => m_Wrapper.m_Basic_AimController;
        public InputAction @Shoot => m_Wrapper.m_Basic_Shoot;
        public InputAction @SwapWeapon1 => m_Wrapper.m_Basic_SwapWeapon1;
        public InputAction @SwapWeapon2 => m_Wrapper.m_Basic_SwapWeapon2;
        public InputAction @SwapWeapon3 => m_Wrapper.m_Basic_SwapWeapon3;
        public InputAction @SwapWeapon4 => m_Wrapper.m_Basic_SwapWeapon4;
        public InputAction @SwapWeapon5 => m_Wrapper.m_Basic_SwapWeapon5;
        public InputActionMap Get() { return m_Wrapper.m_Basic; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BasicActions set) { return set.Get(); }
        public void SetCallbacks(IBasicActions instance)
        {
            if (m_Wrapper.m_BasicActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_BasicActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_BasicActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_BasicActionsCallbackInterface.OnMovement;
                @AimMouse.started -= m_Wrapper.m_BasicActionsCallbackInterface.OnAimMouse;
                @AimMouse.performed -= m_Wrapper.m_BasicActionsCallbackInterface.OnAimMouse;
                @AimMouse.canceled -= m_Wrapper.m_BasicActionsCallbackInterface.OnAimMouse;
                @AimController.started -= m_Wrapper.m_BasicActionsCallbackInterface.OnAimController;
                @AimController.performed -= m_Wrapper.m_BasicActionsCallbackInterface.OnAimController;
                @AimController.canceled -= m_Wrapper.m_BasicActionsCallbackInterface.OnAimController;
                @Shoot.started -= m_Wrapper.m_BasicActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_BasicActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_BasicActionsCallbackInterface.OnShoot;
                @SwapWeapon1.started -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon1;
                @SwapWeapon1.performed -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon1;
                @SwapWeapon1.canceled -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon1;
                @SwapWeapon2.started -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon2;
                @SwapWeapon2.performed -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon2;
                @SwapWeapon2.canceled -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon2;
                @SwapWeapon3.started -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon3;
                @SwapWeapon3.performed -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon3;
                @SwapWeapon3.canceled -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon3;
                @SwapWeapon4.started -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon4;
                @SwapWeapon4.performed -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon4;
                @SwapWeapon4.canceled -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon4;
                @SwapWeapon5.started -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon5;
                @SwapWeapon5.performed -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon5;
                @SwapWeapon5.canceled -= m_Wrapper.m_BasicActionsCallbackInterface.OnSwapWeapon5;
            }
            m_Wrapper.m_BasicActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @AimMouse.started += instance.OnAimMouse;
                @AimMouse.performed += instance.OnAimMouse;
                @AimMouse.canceled += instance.OnAimMouse;
                @AimController.started += instance.OnAimController;
                @AimController.performed += instance.OnAimController;
                @AimController.canceled += instance.OnAimController;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @SwapWeapon1.started += instance.OnSwapWeapon1;
                @SwapWeapon1.performed += instance.OnSwapWeapon1;
                @SwapWeapon1.canceled += instance.OnSwapWeapon1;
                @SwapWeapon2.started += instance.OnSwapWeapon2;
                @SwapWeapon2.performed += instance.OnSwapWeapon2;
                @SwapWeapon2.canceled += instance.OnSwapWeapon2;
                @SwapWeapon3.started += instance.OnSwapWeapon3;
                @SwapWeapon3.performed += instance.OnSwapWeapon3;
                @SwapWeapon3.canceled += instance.OnSwapWeapon3;
                @SwapWeapon4.started += instance.OnSwapWeapon4;
                @SwapWeapon4.performed += instance.OnSwapWeapon4;
                @SwapWeapon4.canceled += instance.OnSwapWeapon4;
                @SwapWeapon5.started += instance.OnSwapWeapon5;
                @SwapWeapon5.performed += instance.OnSwapWeapon5;
                @SwapWeapon5.canceled += instance.OnSwapWeapon5;
            }
        }
    }
    public BasicActions @Basic => new BasicActions(this);
    public interface IBasicActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnAimMouse(InputAction.CallbackContext context);
        void OnAimController(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnSwapWeapon1(InputAction.CallbackContext context);
        void OnSwapWeapon2(InputAction.CallbackContext context);
        void OnSwapWeapon3(InputAction.CallbackContext context);
        void OnSwapWeapon4(InputAction.CallbackContext context);
        void OnSwapWeapon5(InputAction.CallbackContext context);
    }
}
