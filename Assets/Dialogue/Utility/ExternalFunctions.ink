EXTERNAL Unity_Event(eventName)
EXTERNAL Get_State(id)
EXTERNAL Add_State(id, amount)

=== function Unity_Event(eventName)
// Fallback in case actual function is not available.
[ EVENT: {eventName} ]

=== function Get_State(id)
[ GET STATE: {id} ]
~ return 0

=== function Add_State(id, amount)
[ SET STATE: {id} - VALUE: {amount} ]
