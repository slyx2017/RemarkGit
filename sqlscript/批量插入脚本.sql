

  SELECT *
  FROM [dbo].[Sys_CodeType] t
  Where CodeType='HR'

SELECT 'insert into [MCPERP].[dbo].[Sys_Code](ItemCode,ItemName,ItemValue,IsDefault,SortCode,DeleteMark,EnabledMark,Description,CreateDT,CreaterID,CreaterName) values('''+t.ItemCode+''','''+t.ItemName+''','''+t.ItemValue+''','''+Convert(varchar,t.IsDefault)+''','''+Convert(varchar,t.SortCode)+''','''
+Convert(varchar,t.DeleteMark)+''','''+Convert(varchar,t.EnabledMark)+''','''+t.[Description]+''',GetDate(),'''+t.CreaterID+''','''+t.CreaterName+''')'
  FROM [MCPERP_TEST].[dbo].[Sys_Code] t
  Where ItemCode in (
  'HR_0103',
'HR_0104',
'HR_0105',
'HR_0106'
  )
 

  /*

  select * from Sys_Code where ItemName='HRÄ£¿é'

SELECT 'insert into [MCPERP].[dbo].[Sys_CodeType](ItemCode,ItemName,CodeType,DeleteMark,EnabledMark,Description) values('''+t.ItemCode+''','''+t.ItemName+''','''+t.CodeType+''','''+Convert(varchar,t.DeleteMark)+''','''+Convert(varchar,t.EnabledMark)+''','''+t.Description+''');'
  FROM [dbo].[Sys_CodeType] t
  Where CodeType='HR'
  
  SELECT *
  FROM [dbo].[Sys_CodeType] t
  Where CodeType='HR'


  --×Ô¶¯±àÂë
  insert into Sys_AutoCode(CodeName,Remark,CodeLength,CodeRule,NumberLength,CountingCycle,CodePreview,IsSysInner) values
('OA_SP_Task_TaskID','',20,'TK[YYYY][MM][X]',4,'M','',0)
insert into Sys_AutoCodeCounter(CodeName,CountKey,CountValue) values
('OA_SP_Task_TaskID','202001',0)
  */




SELECT 'insert into [MCPERP].[dbo].[Sys_Code](ItemCode,ItemName,ItemValue,IsDefault,SortCode,DeleteMark,EnabledMark,Description,CreateDT,CreaterID,CreaterName) values("'+t.ItemCode+'","'+t.ItemName+'","'+t.ItemValue+'","'+Convert(varchar,t.IsDefault)+'","'+Convert(varchar,t.SortCode)+'","'
+Convert(varchar,t.DeleteMark)+'","'+Convert(varchar,t.EnabledMark)+'","'+t.[Description]+'",GetDate(),"'+t.CreaterID+'","'+t.CreaterName+'")'
  FROM [MCPERP_TEST].[dbo].[Sys_Code] t
  Where ItemCode='HR_0100'
  /*
  */