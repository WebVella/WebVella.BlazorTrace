# v1.0.11
-- FIX

# v1.0.10
-- add ability to review the traces as a chronological log

# v1.0.9
- [[LlamaNL](https://github.com/LlamaNL)] Made Fody dependency transitive to user project can use the library
- addon nugets updated
- trace component name now uses Type.Name 

# v1.0.8
- greatly simplifying the way the library is configured for use
  
# v1.0.7
- FEATURE: trace be decoration component class or methods with [WvBlazorTrace] attribute
- CODE CLEAN: moving the solution file to the /src folder

# v1.0.6
- OPTIMIZATION: memory size calculation optimization for large repeatable objects.
- OPTIMIZATION: adding compression for snapshots in local storage.
- OPTIMIZATION: changing documents to instruct SignalR Hub Message Size increase to 10 MB from the current 1MB, which is needed for bigger projects with large snapshots.

# v1.0.5
- FIX: adding missing JsonIgnores (delete localstorage if problems occur)

# v1.0.4

- FEATURE: Create "Mute" trace functionality to mute tracers based on module, component, component instance, method or custom data
- OPTIMIZATION: Virtualize the result rows for scalable interface
- FEATURE: Create "Clear Current" session functionality to quickly flush all the current tracers.

# v1.0.3
- FEATURE: Support .net8 and .net9 by @jhsheets in #6

# v1.0.0
- FEATURE: Initial Releat
