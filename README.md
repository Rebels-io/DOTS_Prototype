# DOTS Prototype
This is a prototype made to accompany the advisory report written for Rebels by Tim Wolfram between February-July of 2021. The objective was to research DOTS and summarise the findings; these have been written into an advisory report. This report constains advice on the use of DOTS, in what situations DOTS is ideal, and to what extent DOTS should be used by Rebels' Unity team. It also contains a chapter dedicated to the problems I ran into while trying to make this prototype. It contains a few components that can make it easier to understand and/or implement DOTS. For example, the folder Assets/Scripts/DOTS/Hybrid will have a few components that will make it easier to synchronize your GameObjects with Entities.
## Using the prototype
Currently, the prototype is fairly simple. You can walk around using the WASD keys and jump using the space key. The UI contains a single text field that updates based on the amount of Rampage collected. Collecting rampage doesn't do anything currently other than increase the counter of picked up items. 

## Available scripts
The prototype contains a number of simple DOTS systems. For example, the folder Assets/Scripts/DOTS/SineWave shows you how to create a simple data component and Entities.ForEach function.

The folder "Assets/Scripts/DOTS/Hybrid" contains a few classes that are currently necessary to link the old GameObject world to the new DOTS/ECS world. For example, the script UpdateAnimatorComponentVelocity contains an example of how to update a GameObject's animator component with the values of the Unity.Physics DOTS component: PhysicsVelocity (which will be automatically added when using the component "Physics Body").

The script Assets/Scripts/DOTS/RestartLevelSystem.cs shows how to load a new scene; the DOTS world is not considered to be part of a scene, and loading a new scene involves some extra steps. It also shows you how to use a NativeArray to pass data from a worker thread to the main thread. 
The folder "Assets/Scripts/DOTS/Collisions"
## Practices
The syntax of DOTS is very different from regular (MonoBehaviour) Unity code. To get used to this syntax, I recommend starting small. Try to create some simple systems that already exist in the prototype. First, try to create a movement system; this should be relatively simple. Another simple system you can implement to practice DOTS syntax is moving a number of objects using some kind of math function, such as a sine wave. You can use the existing implementations to see an example of how to implement these functionalities, or you can use it to compare to your own code.
Some other features you can practice with that should be relatively simple to implement, but do not exist in this project yet:
- Increasing movement speed or jump force based on amount of Rampage pickups of a single type

Some features that might be more difficult to implement:
- Create a bomb that explodes after a few seconds, killing the player
- Send the event "Jump" to a GameObject Animator component when a player jumps. Note: the implementation will be very different if jumping is handled from the main thread (.Run) vs from a worker thread (.Schedule/.ScheduleParallel)
- Implement a double jump mechanic

## Sources for learning about DOTS
Whenever looking for DOTS information (especially syntax), make sure the source isn't too old. Much has changed about DOTS syntax since the first versions, and search engines like Google will often send you to old tutorials or documentation, where the syntax is much less easy and readable. For DOTS concepts, old sources are usually fine. I'd recommend following these Unity Learn courses: 
Unity Learn, which is currently free for all users, has some courses about DOTS: 
- “Entity Component System” is a compilation of video tutorials which introduce the core concepts of DOTS: the Entity Component System structure, the Job System, and the Burst Compiler.
- “What is DOTS and why is it important?:” is a more detailed introduction to DOTS and its concepts.
“DOTS Best Practices” is an in-depth guide to the best practices DOTS developers should be aware of, and the concepts of multithreading and Data-Oriented Design. It consists of three parts:
  - Understand data-oriented design
  - Data design
  - Implementation and optimization
Unity's DOTS forum is also a great place to get help whenever you're stuck; though the community is relatively small, they are very helpful. Unity Technology's DOTS development team are regularly on the forums as well to answer questions or recieve feedback.
