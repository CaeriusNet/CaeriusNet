﻿using CaeriusNet.Benchmark.Data.Simple;
using CaeriusNet.Benchmark.Workshops.Benchs.ListCapacity.Setup;

namespace CaeriusNet.Benchmark.Workshops.Benchs.ListCapacity;

[MemoryDiagnoser]
public class ListWithCapacityWithOverextendToBench
{
	private static readonly ReadOnlyCollection<SimpleDto> Data = ListCapacityBogusSetup.Faking50KItemsDto;

	private readonly Consumer _consumer = new();

	private readonly List<SimpleDto> _data1 = Data.Take(1).ToList();
	private readonly List<SimpleDto> _data10 = Data.Take(10).ToList();
	private readonly List<SimpleDto> _data100 = Data.Take(100).ToList();
	private readonly List<SimpleDto> _data100000 = Data.ToList();
	private readonly List<SimpleDto> _data10K = Data.Take(10000).ToList();
	private readonly List<SimpleDto> _data1K = Data.Take(1000).ToList();

	[Benchmark]
	public List<SimpleDto> Set_Capacity_With_1_Item_But_Add_1_Item_More()
	{
		var list = new List<SimpleDto>(1);
		list.AddRange(_data1);
		list.AddRange(_data10.Take(1));
		_consumer.Consume(list);
		return list;
	}

	[Benchmark]
	public List<SimpleDto> Set_Capacity_With_10_Items_But_Add_10_Items_More()
	{
		var list = new List<SimpleDto>(10);
		list.AddRange(_data10);
		list.AddRange(_data10);
		_consumer.Consume(list);
		return list;
	}

	[Benchmark]
	public List<SimpleDto> Set_Capacity_With_100_Items_But_Add_100_Items_More()
	{
		var list = new List<SimpleDto>(100);
		list.AddRange(_data100);
		list.AddRange(_data100);
		_consumer.Consume(list);
		return list;
	}

	[Benchmark]
	public List<SimpleDto> Set_Capacity_With_1K_Items_But_Add_1K_Items_More()
	{
		var list = new List<SimpleDto>(1000);
		list.AddRange(_data1K);
		list.AddRange(_data1K);
		_consumer.Consume(list);
		return list;
	}

	[Benchmark]
	public List<SimpleDto> Set_Capacity_With_10000_Items_But_Add_10000_Items_More()
	{
		var list = new List<SimpleDto>(10000);
		list.AddRange(_data10K);
		list.AddRange(_data10K);
		_consumer.Consume(list);
		return list;
	}

	[Benchmark]
	public List<SimpleDto> Set_Capacity_With_100000_Items_But_Add_100000_Items_More()
	{
		var list = new List<SimpleDto>(100000);
		list.AddRange(_data100000);
		list.AddRange(_data100000);
		_consumer.Consume(list);
		return list;
	}
}