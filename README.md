# Saga-Pattern-Orchestration-Eventual-Consistency
The aim of this project is to process the Eventual Consistency by following the best practices with Saga Pattern Orchestration on NETCore.

## Eventual Consistency - Saga Pattern
- A transaction management pattern used in distributed systems and microservices architectures.
- Other systems come into play in sequence based on the success status.
- There is no atomic state.
- In case of possible errors, Compensable Transaction will be activated, and all operations will be rolled back.
   - This ensures data consistency.
- **What is the purpose of Saga?**
   - To introduce a model for managing long-lived and complex transactions in distributed systems and microservices.
   - Aims to make the system more modular and scalable.
- There may be short delays between services.
- There are two different approaches: Events/Choreography and Command/Orchestration.

## Command/Orchestration
- Distributed transactions between services are coordinated with a central controller.
- This controller is referred to as Saga State Machine or alternatively as Saga Orchestrator.
- Saga Orchestrator manages all transactions between services and dictates which operations to perform based on events.
- Saga State Machine keeps track of the application state related to user requests, interprets it, and applies compensatory operations when necessary.
- Easily applicable in systems with multiple services.
- Provides centralized control.
- No need for services to be aware of any information about each other.
  
### State Machine Class
- The class that provides us with the structure of the state machine. It plays a central role in determining states, events, and specific behaviors.
- Manages distributed transactions.
- To be a state machine class, it needs to implement the `MassTransitStateMachine<T>` interface.
- The generic `T` specified is an instance of a State.

### State Instance
- A class representing the data of a State Machine.
- For example, each order request with different data will be stored as a state instance in the database.
- To be a state instance, it needs to implement the `SagaStateMachineInstance` interface.
- The **CorrelationId** is used to distinguish between these two requests.

### State DbContext
- The context object that manages the database where State Machine data is stored.
- Derives from the abstract class `SagaDbContext`.

### State Map
- The class that allows for validation settings of properties in the saved State Instance in the database.
- Derives from the abstract class `SagaClassMap<T>`.
- Generic parameter `T` is a State Instance.

### Four Events in Total
1. **Triggering Event**
   - An event that triggers the corresponding service.
  
2. **Successful Event**
   - An event indicating that the operation has been successfully completed.

3. **Failure Event**
   - An event indicating that the operation has failed.

4. **Compensable Event**
   - An event indicating the reversal of the performed operation.
