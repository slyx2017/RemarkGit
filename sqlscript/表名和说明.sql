SELECT A.name AS tablename,B.name AS columnname,C.value AS description ,d.name as datatype 
FROM sys.tables A 
INNER JOIN sys.columns B ON B.object_id = A.object_id 
LEFT JOIN sys.extended_properties C ON C.major_id = B.object_id AND C.minor_id = B.column_id 
Left join sys.types d on d.user_type_id=b.user_type_id

select a.name,b.value as tabledesc 
from sys.tables a
left join sys.extended_properties b on b.minor_id=0 and b.major_id=a.object_id
where a.type='U'
order by a.name
