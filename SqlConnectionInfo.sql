SP_WHO2

select * from sys.configurations
where name ='user connections'

SELECT oPC.cntr_value AS connection_count
FROM sys.dm_os_performance_counters oPC
WHERE 
	(
		oPC.[object_name] = 'SQLServer:General Statistics'
			AND oPC.counter_name = 'User Connections'
	)
ORDER BY 1;