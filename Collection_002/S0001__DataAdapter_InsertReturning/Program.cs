using System;
using System.Data;
using System.Data.OleDb;

namespace Sample_0001{
////////////////////////////////////////////////////////////////////////////////
//class Program

class Program
{
 private const int c_nRows=5;

 private const string c_cn_str
  ="provider=LCPI.IBProvider.3;"
  +"location=localhost:d:\\database\\ibp_test_fb25_d3.gdb;"
  +"user id=gamer;"
  +"password=vermut;";

 //Table:
 // TEST_SEQUENTIAL_MOVE
 //
 //Columns:
 // ID BIGINT [NOT NULL] [PRIMARY KEY]
 //
 //Triggers
 // BEFORE INSERT. IF NEW.ID is NULL THEN NEW.ID=GEN_ID(...,1);

 static int Main()
 {
  int resultCode=0;

  try // [catch]
  {
   OleDbConnection  cn=null;
   OleDbTransaction tr=null;
   OleDbDataAdapter da=null;
   DataTable        table=null;

   try // [finally]
   {
    cn=new OleDbConnection(c_cn_str);

    cn.Open();

    //--------------------------------------
    tr=cn.BeginTransaction(IsolationLevel.RepeatableRead);

    //-------------------------------------- create data_adapter
    da=new OleDbDataAdapter();

    //-------------------------------------- prepare insert command

    //use explicit marker for OUT-parameter
    da.InsertCommand=new OleDbCommand("insert into TEST_SEQUENTIAL_MOVE (ID) values (?)\n"
                                      +"returning ID into ?",
                                      cn,tr);
    {
     var cmd_params=da.InsertCommand.Parameters;

     cmd_params.Add(Helper__CreateParam(ParameterDirection.Input,
                                        OleDbType.BigInt,
                                        "ID"));

     cmd_params.Add(Helper__CreateParam(ParameterDirection.Output,
                                        OleDbType.BigInt,
                                        "ID"));
    }//local

    //-------------------------------------- build table
    table=new DataTable();

    table.Columns.Add("ID",typeof(Int64));

    //-------------------------------------- fill table
    for(int i=0;i<c_nRows;++i)
     table.Rows.Add(table.NewRow());

    Helper__PrintTable(table);

    //-------------------------------------- update
    Console.WriteLine("");
    Console.WriteLine("Update");

    da.Update(table);

    //--------------------------------------
    Console.WriteLine("");
    Helper__PrintTable(table);

    //-------------------------------------- commit
    tr.Commit();
   }
   finally
   {
    Helper__Dispose(table);
    Helper__Dispose(da);
    Helper__Dispose(tr);
    Helper__Dispose(cn);
   }//finally
  }
  catch(Exception e)
  {
   resultCode=1;

   Console.WriteLine("");
   Console.WriteLine("ERROR: {0} - {1}",e.Source,e.Message);
  }//catch

  return resultCode;
 }//Main

 //-----------------------------------------------------------------------
 private static void Helper__PrintTable(DataTable table)
 {
  for(int i=0,_c=table.Rows.Count;i!=_c;++i)
  {
   Console.WriteLine("Rows[{0}]={1} [{2}]",
                     i,
                     Helper__CStrNE(table.Rows[i][0]),
                     table.Rows[i].RowState.ToString());
  }//for i
 }//Helper__PrintTable

 //-----------------------------------------------------------------------
 private static OleDbParameter Helper__CreateParam(ParameterDirection direction,
                                                   OleDbType          type,
                                                   string             source)
 {
  var param=new OleDbParameter();

  param.Direction   =direction;
  param.OleDbType   =type;
  param.SourceColumn=source;

  return param;
 }//Helper__CreateParam

 //-----------------------------------------------------------------------
 private static string Helper__CStrNE(object v)
 {
  if(Object.ReferenceEquals(v,null))
   return "#NULL";

  if(DBNull.Value.Equals(v))
   return "#DBNULL";

  return v.ToString();
 }//Helper__CStrNE

 //-----------------------------------------------------------------------
 private static void Helper__Dispose(IDisposable obj)
 {
  if(!Object.ReferenceEquals(obj,null))
   obj.Dispose();
 }//Helper__Dispose

 //-----------------------------------------------------------------------
 private static void Helper__Dispose(OleDbDataAdapter da)
 {
  if(Object.ReferenceEquals(da,null))
   return;

  Helper__Dispose(da.InsertCommand);

  da.Dispose();
 }//Helper__Dispose
};//class Program

////////////////////////////////////////////////////////////////////////////////
}//namespace Sample_0001
