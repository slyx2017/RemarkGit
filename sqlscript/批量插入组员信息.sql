SELECT 'insert into [MCPERP].[dbo].[HR_PF_TeamMember](MemberID,TeamID,EmployeeNo,EmployeeName,PostID,PostName,Percentage,Bonus,EmployeeType,IsCalc,IsActive,CreaterID,CreaterName,CreateDT,UpdaterID,UpdaterName,UpdateDT) 
values(LOWER(NEWID()),''3dff7306-ee87-4e9f-a264-810fac5606ec'','''+e.EmployeeNo+''','''+e.FullNameCN+''','''+p.PositionID+''','''+p.PositionName+''',''0.0483'',''0.00'',NULL,''TRUE'',''TRUE'',
''U12592'',''��־��'',GetDate(),''U12592'',''��־��'',GetDate())'
  FROM MDT_Employee e 
  left join MDT_Position p on p.PositionID=e.PositionID
  left join MDT_Department d on d.DepartmentID=e.DepartmentID Where d.DepartmentName='���DC��' and e.FullNameCN in (
  '����',
  '��ȫǿ',
  '���ï',
  '�����',
  '����Ⱥ',
  '�ŵ���',
  '�����',
  '«��ΰ',
  '������',
  '�����',
  '֣�״�',
  '������',
  '�½���',
  '½��',
  '����',
  '����ΰ',
  '�����',
  '������',
  '���ʢ',
  '۳ѧ��'
  )















































































  /*


  select * from HR_PF_Team
  where TeamCode='TJDC06'

select e.EmployeeNo,e.FullNameCN,p.PositionID,p.PositionName
  FROM MDT_Employee e 
  left join MDT_Position p on p.PositionID=e.PositionID
  left join MDT_Department d on d.DepartmentID=e.DepartmentID
  where d.DepartmentName='���DC��' and FullNameCN in (
  '��˫',
  '�����',
  '�־���',
  '������',
  '�ܽ���',
  '���ܸ�',
  '��С��',
  'Фˮ��',
  '�º��',
  '������',
  '������',
  'κ־��',
  '���½�',
  '���һ�'
  )

  select * from MDT_Department

  */
