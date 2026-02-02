using System;
using System.Collections.Generic;
using System.Text;

namespace Cvnet10Base;



public class ParamQuery {
	public Type TableType { get; set; } = typeof(string);
	public Type ResultType { get; set; } = typeof(string);
	public string? Where { get; set; }
	public string? Order { get; set; }
	public object[] Parameters { get; set; } = new object[0];

}

public class ParamExecute {
	public Type TableType { get; set; } = typeof(string);

	public ParamExecuteType ExecuteType { get; set; } = ParamExecuteType.Insert;

	public long Id { get; set; } = 0;

	public string JsonData { get; set; } = "{}";

}

public enum ParamExecuteType {
	Insert,
	Update,
	Delete
}




