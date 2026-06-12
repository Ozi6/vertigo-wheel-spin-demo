/*
I NORMALLY DO NOT PUSH THIS INTO MY REPOSITORIES, BUT SINCE THIS IS ALSO AN EVALUATION OF MY DESIGN PROCESS I WOULD LIKE TO SHARE IT AS WELL.
THIS DOCUMENT WAS WRITTEN BEFORE ANY CODE WAS IMPLEMENTED, AND SERVED AS THE BLUEPRINT FOR THE ENTIRE DEVELOPMENT PROCESS. IT IS NOT A COMMENT ON ANY PARTICULAR CLASS OR METHOD, BUT RATHER A HIGH-LEVEL ARCHITECTURAL OVERVIEW OF THE ENTIRE SYSTEM.




Wheel of Fortune System Design Document

Overview
A mobile gambling loop game where the player progresses through zones, spinning a wheel at each one. Rewards accumulate until the player either collects them voluntarily or loses them to a bomb. The architecture prioritizes testability, editability from the Unity inspector, and clean separation between game logic and presentation.

Architectural Style
The system follows a layered architecture with strict dependency direction: outer layers depend on inner layers, never the reverse. Unity-specific code is confined to the outermost layer. All cross-layer communication flows through interfaces and an event bus, so no layer has direct knowledge of another layer's concrete types.
The pattern set in use is State for game flow, Strategy for spin behavior, Observer for decoupled event broadcasting, Command for player actions, Factory Method for wheel construction, and Facade/Mediator for the central controller. SOLID principles govern every boundary decision.

Layers
All configuration should live in ScriptableObjects so designers can modify wheel contents, zone thresholds, and reward tiers without touching code. Nothing in this layer has behavior.
Domain should contain C# models, structs, and enums. No MonoBehaviour, no Unity API. This layer will be fully unit-testable in isolation.
Services contain all game rules and business logic. Each service implements one interface and has one responsibility. Services communicate with each other only through the event bus, never by direct reference.
Controller should be the single orchestrator of the system. It should hold references to all service interfaces and all view interfaces. It owns the state machine and routes events into state transitions. It knows what is happening but not how anything is rendered or calculated.
Views are pure presentation. They receive data and display it. They know nothing about services or game rules. Each view implements a narrow interface so the controller never depends on MonoBehaviour directly.
Installer is the composition root. It runs at startup, constructs all concrete implementations, and injects them into the controller. It is the only place in the codebase where concrete types are referenced by name together.

Data Layer: ScriptableObjects
A RewardItem Scriptable Object should represent a single reward type. It carries an identifier, an icon sprite, a numeric value, and a tier number that scales with zone depth.
WheelConfig SO should describe one wheel. It holds an ordered list of slice definitions, where each slice references a RewardItemSO and carries a weight. One index in the list is designated as the bomb slot. The bomb slot is absent entirely in safe and super zone configs.
ZoneConfig SO should pair a zone type with the wheel config that should be used at that zone. Designers should be able to assign these in the inspector.
GameSettingsSO shall hold global numeric parameters: the safe zone interval (default 5), the super zone interval (default 30), and the starting revive cost.

Domain Layer
ZoneType is an enum with three values: Normal, Safe, and Super.
SpinResult is a value struct carrying a reference to the landed RewardItemSO and a boolean flag indicating whether the result was a bomb.
CollectedRewards is a model class that holds the player's current session inventory as a list of RewardItemSO references. It has no logic beyond storage.
ZoneProgressModel is a snapshot of zone state at a given moment: the current zone number and its derived ZoneType. It is produced by ZoneService and passed around as an event payload.

Interface Layer
Every cross-layer dependency is expressed as an interface. This is the ISP and DIP enforcement point.
IZoneService exposes three operations: getting the current zone type, advancing to the next zone and receiving a ZoneProgressModel back, and querying whether the player is currently allowed to leave.
ISpinService exposes one operation: executing a spin against a given WheelConfigSO and returning a SpinResult.
IRewardService exposes collecting a single item into the session inventory, clearing the entire inventory, and retrieving the current state of the inventory.
IEventBus exposes publishing a typed payload, subscribing a typed handler, and unsubscribing a typed handler. The type parameter is the payload type, so subscriptions are self-documenting.
IWheelView exposes setting up the visual slices from a data array and triggering a spin animation to a target slice index, with a completion callback.
IHudView exposes updating the zone indicator display and updating the collected rewards display.
IDialogView exposes showing the bomb screen with revive and give-up options, and showing the collect confirmation screen.

Event Bus
The EventBus is a plain C# class implementing IEventBus. Internally it maintains a dictionary keyed by payload type, mapping to a multicast delegate. Publish invokes all registered handlers for that type. Subscribe and Unsubscribe add and remove handlers.
All cross-service side effects are expressed as events rather than direct calls. This ensures services remain decoupled and the controller can react to outcomes without being threaded through service internals.
Event types in use:
OnSpinCompleted carries a SpinResult. Published by SpinService after a spin is resolved.
OnZoneAdvanced carries a ZoneProgressModel. Published by ZoneService after the zone counter increments.
OnBombHit carries no payload. Published by RewardService after the inventory is cleared as a result of a bomb.
OnRewardCollected carries the current CollectedRewards snapshot. Published by RewardService after an item is added.
OnPlayerLeft carries the final CollectedRewards. Published by GameController when the player chooses to collect and exit.

Spin Strategy
SpinService does not contain spin logic directly. It delegates to an IWheelSpinStrategy, which has a single operation: given a WheelConfigSO, return the index of the winning slice.
RandomSpinStrategy performs a uniform random selection across all slices. This is used for Normal and Safe zones.
WeightedSpinStrategy performs a weighted random selection using the weight values on each slice definition. This is used for the Super zone golden wheel to allow tuning of special reward probabilities.
GameController is responsible for setting the correct strategy on SpinService before each spin, based on the current ZoneType.

State Machine
GameController owns the state machine. It holds a reference to the current IGameState and calls TransitionTo when a state change is needed. TransitionTo calls Exit on the outgoing state and Enter on the incoming state.
IdleState is the resting state between spins. On entry it updates the HUD and enables the spin and collect buttons. It exposes CanSpin and CanCollect guards that other systems query before issuing commands. CanCollect returns true only when the zone type is Safe or Super.
SpinningState drives the wheel animation. On entry it disables all player input, calls ISpinService.Spin with the current config, and then calls IWheelView.SpinTo with the result index. When the animation completes it transitions to RewardState or BombState depending on the result.
RewardState calls IRewardService.Collect with the landed item, then calls IZoneService.Advance. It transitions back to IdleState once the reward display is complete.
BombState calls IRewardService.ClearAll, then calls IDialogView.ShowBomb. If the player chooses to revive, ReviveCommand is executed and the state transitions back to IdleState. If the player gives up, GiveUpCommand is executed and the game resets.
CollectState publishes OnPlayerLeft with the current inventory, then shows the collect confirmation dialog. On confirmation it resets all services and transitions to IdleState at zone one.

Command Pattern
Every player initiated action is a command object implementing ICommand with a single Execute method. The controller creates and holds command instances. Buttons are assigned their command references in OnValidate, entirely through code, with no Unity OnClick inspector wiring.
SpinCommand checks IdleState.CanSpin before transitioning to SpinningState.
CollectCommand checks IZoneService.CanPlayerLeave before transitioning to CollectState.
ReviveCommand deducts the revive cost from the player's currency, then signals BombState to proceed to IdleState.
GiveUpCommand signals BombState to reset the game entirely.

Factory Layer
WheelFactory takes a ZoneType and selects the matching ZoneConfigSO from an inspector assigned array. It returns a configured wheel view by instructing SliceFactory to populate it.
SliceFactory takes a slice data array and a parent transform. It instantiates one wheel slice prefab per entry, assigns the icon sprite and value label using the _value-suffixed field names, and returns the populated set.
This means adding a new zone variant requires only a new ZoneConfigSO asset and a new WheelConfigSO asset. No code changes.

Controller
GameController is a MonoBehaviour that is fully initialized by GameInstaller via an Init method, not by Unity's Awake or Start serialization. It holds all service and view interfaces as private fields. It subscribes to all relevant EventBus events on initialization and routes them to state transitions. It exposes no public state, all external interaction happens through commands.

Installer
GameInstaller runs in Awake before any other MonoBehaviour. It constructs EventBus, ZoneService, SpinService, RewardService, WheelFactory, and SliceFactory. It locates the view MonoBehaviours in the scene using GetComponentInChildren against the interface types. It calls GameController.Init with all constructed dependencies. After Init returns, the game is in a valid state and GameController transitions to IdleState.

UI Rules
Canvas scaler is set to Expand mode. All text uses TextMeshPro. All inspector driven value fields end with _value in their GameObject names. GameObject naming goes from general to specific: ui_image_spin_silver_value, ui_text_zone_value, and so on. Image components that do not receive input have RaycastTarget disabled and Maskable disabled. Animators are placed on child transforms, never on root transforms. Anchors and pivots are set correctly per element so layouts hold across 20:9, 16:9, and 4:3. Sliced sprites are used throughout. No images are stretched.

Implementation Stages
Stage 1: Foundation
Create all ScriptableObject definitions and populate them with placeholder content. Implement the domain models and enums. Implement EventBus and verify it with unit tests. Nothing Unity-specific beyond the ScriptableObject declarations.

Stage 2: Services
Implement ZoneService, SpinService, and RewardService against their interfaces. Implement RandomSpinStrategy and WeightedSpinStrategy. Unit test all services and strategies in isolation with no MonoBehaviour involvement.

Stage 3: State Machine
Implement all five states with stubbed transitions. Wire GameController to own and drive the state machine. Verify the full happy path (Idle -> Spinning -> Reward -> Idle) and the bomb path (Idle -> Spinning -> Bomb -> reset) using the EventBus.

Stage 4: Commands
Implement all four command classes with their guards. Connect them to the relevant state transitions in GameController. Verify that guards correctly block commands when preconditions are not met.

Stage 5: Factories and Scene
Implement WheelFactory and SliceFactory. Build the Unity scene hierarchy with correct Canvas settings, naming conventions, anchors, and pivot points. Implement GameInstaller and verify clean boot with all dependencies resolved.

Stage 6: Views and Presenters
Implement WheelPresenter with DOTween spin animation. Implement HudPresenter and DialogPresenter. Apply all UI naming and component configuration rules. Verify each view in isolation by calling its interface methods directly.

Stage 7: Zone Variants
Wire the three wheel types: normal silver wheel with bomb, safe silver wheel without bomb, golden super wheel with weighted rewards. Enforce the leave restriction in CanPlayerLeave so the collect button only appears in Safe and Super zones.

Stage 8: Polish and Delivery
Set up Sprite Atlas. Audit all anchors and pivots against 20:9, 16:9, and 4:3. Add the optional revive currency system if time allows. Build the APK and publish to GitHub as a release.
*/