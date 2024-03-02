ToDo:
- Parameterizing: enumerate_all_solutions:true linearization_level:2
- Console logging
- Input validation

Activities from old database:

```
SELECT TOP (1000) *
FROM [dbo].[eventlocations]
LEFT JOIN [dbo].[eventlocationactivities] ON [eventlocationactivities].IdEventLocation = [eventlocations].Id
LEFT JOIN [dbo].[activities] ON [eventlocationactivities].IdActivity = [activities].Id
WHERE [eventlocations].IdEvent = 1102
```