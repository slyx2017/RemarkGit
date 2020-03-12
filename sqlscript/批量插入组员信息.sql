SELECT 'insert into [MCPERP].[dbo].[HR_PF_TeamMember](MemberID,TeamID,EmployeeNo,EmployeeName,PostID,PostName,Percentage,Bonus,EmployeeType,IsCalc,IsActive,CreaterID,CreaterName,CreateDT,UpdaterID,UpdaterName,UpdateDT) 
values(LOWER(NEWID()),''3dff7306-ee87-4e9f-a264-810fac5606ec'','''+e.EmployeeNo+''','''+e.FullNameCN+''','''+p.PositionID+''','''+p.PositionName+''',''0.0483'',''0.00'',NULL,''TRUE'',''TRUE'',
''U12592'',''汪志永'',GetDate(),''U12592'',''汪志永'',GetDate())'
  FROM MDT_Employee e 
  left join MDT_Position p on p.PositionID=e.PositionID
  left join MDT_Department d on d.DepartmentID=e.DepartmentID Where d.DepartmentName='天津DC部' and e.FullNameCN in (
  '邢亮',
  '李全强',
  '孙金茂',
  '邹玉恒',
  '寇永群',
  '门殿林',
  '韩振国',
  '芦春伟',
  '范红星',
  '范红兵',
  '郑兆达',
  '王文友',
  '陈金朋',
  '陆亮',
  '李猛',
  '王晓伟',
  '李佳旺',
  '刘丽超',
  '李鸿盛',
  '鄢学清'
  )















































































  /*


  select * from HR_PF_Team
  where TeamCode='TJDC06'

select e.EmployeeNo,e.FullNameCN,p.PositionID,p.PositionName
  FROM MDT_Employee e 
  left join MDT_Position p on p.PositionID=e.PositionID
  left join MDT_Department d on d.DepartmentID=e.DepartmentID
  where d.DepartmentName='天津DC部' and FullNameCN in (
  '李双',
  '李款如',
  '林军和',
  '刘敏超',
  '周健安',
  '苟能刚',
  '贺小军',
  '肖水林',
  '陈红军',
  '黄梦秋',
  '王继祥',
  '魏志富',
  '陈新杰',
  '连忠华'
  )

  select * from MDT_Department

  */
