using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

/// <summary>
/// 유니티 에디터로부터 콜백을 받아 현재 임포터에 등록된 스크립트 클래스들을 탐색하고
/// 지정한 어트리뷰트에 따라 스크립트 초기화 순서를 프로젝트에 적용시키는 정적 클래스
/// </summary>
public static class ExecutionOrderAttributeEditor
{
	#region <Structs>

	/// <summary>
	/// 특정 스크립트의 초기화 우선도를 기술하는 튜플
	/// </summary>
	struct ScriptExecutionOrderDefinition
	{
		/// <summary>
		/// 노드 주데이터
		/// </summary>
		public MonoScript script { get; set; }
		
		/// <summary>
		/// 해당 노드의 초기화 순서
		/// </summary>
		public int order { get; set; }
	}

	/// <summary>
	/// 특정 스크립트가 다른 스크립트에 종속되어있을 때, 초기화 우선도를 기술하는 튜플
	/// </summary>
	struct ScriptExecutionOrderDependency
	{
		/// <summary>
		///  해당 노드가 종속된 스크립트
		/// </summary>
		public MonoScript AttributeTargetType { get; set; }
		
		/// <summary>
		/// 해당 노드를 기술하는 스크립트
		/// </summary>
		public MonoScript AttributeMasterType { get; set; }
		
		/// <summary>
		/// 마스터 슬레이브 노드 사이의 스크립트 초기화 순서 차이
		/// </summary>
		public int orderDelta { get; set; }
	}

	#endregion

	#region <Classess>

	/// <summary>
	/// 정적 내부 클래스
	/// </summary>
	private static class Graph
	{
		/// <summary>
		/// 특정 스크립트와 우선도를 가지는 구조체
		/// </summary>
		public struct Edge
		{
			/// <summary>
			/// 해당 노드가 가진 주데이터, 기본적으로 스크립트 타입은 유니크하므로 노드로 사용하기 적합하다.
			/// </summary>
			public MonoScript node;
			
			/// <summary>
			/// 해당 노드의 우선도
			/// </summary>
			public int weight;
		}

		/// <summary>
		/// 지정한 종속성 및 우선도 정보를 그래프 컬렉션으로 초기화하여 리턴하는 메서드
		/// 해당 그래프 생성 과정에서 관련 어트리뷰트를 정의받지 않은 일반 스크립트도, 종속성의 대상이 된다면 해당 그래프에 포함될 수 있다.
		/// 리턴타입은 [스크립트, 해당 스크립트에 의존하고 있는 edge] 컬렉션.
		/// </summary>
		/// <param name="definitions">각 스크립트 타입은 유니크하지만, 동시에 여러 어트리뷰트를 가질 수 있기에 같은 스크립트 타입이 들어 있을 수 있다.</param>
		/// <param name="dependencies">특정 스크립트가 다른 스크립트를 기준으로 하는 종속성을 가져야 하는 경우, 해당 종속성을 튜플로 정의한 리스트</param>
		public static Dictionary<MonoScript, List<Edge>> Create(List<ScriptExecutionOrderDefinition> definitions, List<ScriptExecutionOrderDependency> dependencies)
		{
			// 리턴값
			var graph = new Dictionary<MonoScript, List<Edge>>();

			// 종속성에 따라 리턴값에 노드 정보를 초기화 시켜준다.
			foreach(var dependency in dependencies)
			{
				var target = dependency.AttributeTargetType;
				var master = dependency.AttributeMasterType;
				List<Edge> edges;
				if(!graph.TryGetValue(target, out edges))
				{
					edges = new List<Edge>();
					graph.Add(target, edges);
				}
				edges.Add(new Edge() { node = master, weight = dependency.orderDelta });
				if(!graph.ContainsKey(master))
				{
					graph.Add(master, new List<Edge>());
				}
			}

			// 종속성은 가지지 않고 순서만 가지는 그 외 스크립트를 초기화 시켜준다.
			foreach(var definition in definitions)
			{
				var node = definition.script;
				if(!graph.ContainsKey(node))
				{
					graph.Add(node, new List<Edge>());
				}
			}

			return graph;
		}

		/// <summary>
		/// Create에 의해 생성된 그래프 컬렉션의 스크립트 노드들이 순환하고 있는지 검증하는 메서드의 진입점
		/// </summary>
		/// <param name="graph">Create 메서드에 의해 생성 및 초기화된 그래프 컬렉션</param>
		public static bool IsCyclic(Dictionary<MonoScript, List<Edge>> graph)
		{
			var visited = new Dictionary<MonoScript, bool>();
			var inPath = new Dictionary<MonoScript, bool>();
			foreach(var node in graph.Keys)
			{
				visited.Add(node, false);
				inPath.Add(node, false);
			}

			// 재귀를 통해 지정한 노드가 순환하고 있는지 검증하여 리턴
			foreach(var node in graph.Keys)
				if(IsCyclicRecursion(graph, node, visited, inPath))
					return true;
			
			return false;
		}

		/// <summary>
		/// Create에 의해 생성된 그래프 컬렉션의 스크립트 노드들이 순환하고 있는지 검증하는 재귀 메서드
		/// </summary>
		/// <param name="graph">Create 메서드에 의해 생성 및 초기화된 그래프 컬렉션</param>
		/// <param name="node">탐색 시작 노드</param>
		private static bool IsCyclicRecursion(Dictionary<MonoScript, List<Edge>> graph, MonoScript node, Dictionary<MonoScript, bool> visited, Dictionary<MonoScript, bool> inPath)
		{
			// 아직 방문하지 않은 노드인 경우에
			if(!visited[node])
			{
				// 해당 노드의 방문 플래그를 세트해준다.
				visited[node] = true;
				
				// 해당 노드가 현재 순환 여부를 체크 중이라는 정보를 플래그에 세트해준다.
				inPath[node] = true;

				// 해당 노드와 종속성을 가지는 노드에 대해서 재귀호출해준다.
				foreach(var edge in graph[node])
				{
					var succ = edge.node;
					// 종속성을 가지는 노드들 중에 순환하는 노드가 있다면, 해당 노드 역시 순환하는 노드가된다.
					if(IsCyclicRecursion(graph, succ, visited, inPath))
					{
						// 순환 체크가 종료되었으므로 관련 플래그를 해제하고, 종속성을 가진 노드에서 순환을 찾아냈으므로(정확히는 아래의 'else if(inPath[node])' 블럭)\
						// true를 리턴한다.
						inPath[node] = false;
						return true;
					}
				}

				// 순환 체크가 종료되었으므로 관련 플래그를 해제하고, 순환을 찾지 못했으므로 false를 리턴한다.
				inPath[node] = false;
				return false;
			}
			// 지정한 노드가 이미 순환 체크 중이라는 것은, 해당 노드에서 시작한 재귀가 해당 노드로 돌아왔음을 의미하므로 순환이 증명된 것.
			// 따라서 true를 리턴한다.
			else if(inPath[node])
			{
				return true;
			}
			// 그 외의 경우
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Create 메서드로 생성된 그래프에서 한번도 edge로 선정되지 않은 노드를 리스트로 리턴한다.
		/// edge로 한번도 선정되지 않았다는 것은 해당 노드가 다른 노드로의 종속성을 가지지 않음을 의미한다.
		/// </summary>
		public static List<MonoScript> GetRoots(Dictionary<MonoScript, List<Edge>> graph)
		{
			var degrees = new Dictionary<MonoScript, int>();
			foreach(var node in graph.Keys)
			{
				degrees.Add(node, 0);
			}

			// KVP ~ key value pair
			foreach(var kvp in graph)
			{
				var node = kvp.Key;
				var edges = kvp.Value;
				foreach(var edge in edges)
				{
					var succ = edge.node;
					degrees[succ]++;
				}
			}

			var roots = new List<MonoScript>();
			foreach(var kvp in degrees)
			{
				var node = kvp.Key;
				int degree = kvp.Value;
				if(degree == 0)
					roots.Add(node);
			}
			return roots;
		}

		/// <summary>
		/// graph의 종속 정보에 따라 values 컬렉션의 레코드인 각 스크립트 타입의 초기화 우선도를 갱신하는 메서드
		/// graph에는 지정한 어트리뷰트를 상속받지 않는 일반 스크립트가 포함될 수 있다.
		/// </summary>
		public static void PropagateValues(Dictionary<MonoScript, List<Edge>> graph, Dictionary<MonoScript, int> values)
		{
			var queue = new Queue<MonoScript>();
			foreach(var node in values.Keys)
				queue.Enqueue(node);

			while(queue.Count > 0)
			{
				var node = queue.Dequeue();
				// 현재 선택된 스크립트의 가중치 값
				int currentValue = values[node];

				// 현재 선택된 스크립트를 의존하는 스크립트 edge 에 대해서, (즉, 루트 노드는 해당 반복문에 진입하지 못한다.)
				foreach(var edge in graph[node])
				{
					var succ = edge.node;
					// 해당 edge 가중치에 해당 edge가 의존하는 노드의 가중치를 더한 값을 newValue로 정의한다.
					var newValue = currentValue + edge.weight;
					
					// CASE # 1 : 지정한 edge가 특정한 우선도 값을 가지는지 체크하는 플래그
					// 기본적으로 모든 노드는 우선도나 종속성에 의한 offset 둘 중 하나의 값만 가지지만
					// 해당 메서드를 통해 루트 노드로부터 전체 노드를 향해 종속성 offset을 계산해 나가면서
					// 우선도가 선정되게 된다. 
					// 해당 과정에서 while문의 '이전' 과정을 통해 우선도가 결정된 값이 발생할 수 있고, 때문에 prevValue이다.
					var prevValue = 0;
					bool hasPrevValue = values.TryGetValue(succ, out prevValue);

					// CASE # 2 : 어떤 종속성을 가지는 노드의 offset이 음수라면 해당 노드는 마스터 노드보다 먼저 초기화가 되어야 한다.
					// 반대로 양수라면 늦게 초기화가 되어야 한다.
					//
					// 하나의 스크립트는 복수의 스크립트를 의존할 수 있기 때문에 prevValue를 고려해서,
					// 가장 우선도가 낮은 (값이 큰) 값을 선정한다.
					bool newValueBeyond = (edge.weight > 0) ? (newValue > prevValue) : (newValue < prevValue);
					
					if(!hasPrevValue || newValueBeyond)
					{
						values[succ] = newValue;
						queue.Enqueue(succ);
					}
				}
			}
		}
	}

	#endregion

	#region <Methods>
	
	/// <summary>
	/// 현재 유니티 에디터에서 관리중인 타입들 중에, 유니티 엔진 계열 타입들을 컬렉션으로 리턴하는 메서드
	/// </summary>
	private static Dictionary<Type, MonoScript> GetTypeDictionary()
	{
		var types = new Dictionary<Type, MonoScript>();
		var scripts = MonoImporter.GetAllRuntimeMonoScripts();
		
		foreach(var script in scripts)
		{
			var type = script.GetClass();
			if(IsTypeValid(type))
			{
				if(!types.ContainsKey(type))
					types.Add(type, script);
			}
		}

		return types;
	}

	/// <summary>
	/// 지정한 타입이 유니티 엔진 계열(MonoBehaviour, ScriptableObject) 클래스 타입인지 체크하는 논리 메서드
	/// </summary>
	private static bool IsTypeValid(Type type)
	{
		if(type != null)
			return type.IsSubclassOf(typeof(MonoBehaviour)) || type.IsSubclassOf(typeof(ScriptableObject));
		else
			return false;
	}

	/// <summary>
	/// GetTypeDictionary 로 부터 생성된 현재 유니티 에디터에서 추적중인 유니티 엔진 계열 타입 컬렉션 중에서 hasExecuteAfterAttribute, hasExecuteBeforeAttribute 어트리뷰트를
	/// 가지는 타입을 리스트화여 리턴하는 메서드
	/// 어트리뷰트 우선도는 hasExecuteBeforeAttribute > hasExecuteAfterAttribute > ExecutionOrderAttribute 순서가 된다.
	/// 하나의 스크립트가 동일한 종속성 어트리뷰트를 복수 가지는 경우, 가지는 만큼 리스트에 추가된다.
	/// </summary>
	private static List<ScriptExecutionOrderDependency> GetExecutionOrderDependencies(Dictionary<Type, MonoScript> types)
	{
		var list = new List<ScriptExecutionOrderDependency>();

		// 모든 타입에 대해서,
		foreach(var kvp in types)
		{
			var type = kvp.Key;
			var script = kvp.Value;
			
			// 지정한 타입에 ExecutionOrderAttribute 계열 어트리뷰트가 붙어있는지 검증한다.
			bool hasExecutionOrderAttribute = Attribute.IsDefined(type, typeof(ExecutionOrderAttribute));
			bool hasExecuteAfterAttribute = Attribute.IsDefined(type, typeof(ExecuteAfterAttribute));
			bool hasExecuteBeforeAttribute = Attribute.IsDefined(type, typeof(ExecuteBeforeAttribute));

			// 동시에 2개 이상의 어트리뷰트를 보유한 타입에 대한 이벤트핸들링
			if(hasExecuteAfterAttribute)
			{
				if(hasExecutionOrderAttribute)
				{
					Debug.LogError(string.Format("Script {0} has both [ExecutionOrder] and [ExecuteAfter] attributes. Ignoring the [ExecuteAfter] attribute.", script.name), script);
					continue;
				}

				var attributes = (ExecuteAfterAttribute[])Attribute.GetCustomAttributes(type, typeof(ExecuteAfterAttribute));
				foreach(var attribute in attributes)
				{
					if(attribute.orderIncrease < 0)
					{
						Debug.LogError(string.Format("Script {0} has an [ExecuteAfter] attribute with a negative orderIncrease. Use the [ExecuteBefore] attribute instead. Ignoring this [ExecuteAfter] attribute.", script.name), script);
						continue;
					}

					if(!attribute.targetType.IsSubclassOf(typeof(MonoBehaviour)) && !attribute.targetType.IsSubclassOf(typeof(ScriptableObject)))
					{
						Debug.LogError(string.Format("Script {0} has an [ExecuteAfter] attribute with targetScript={1} which is not a MonoBehaviour nor a ScriptableObject. Ignoring this [ExecuteAfter] attribute.", script.name, attribute.targetType.Name), script);
						continue;
					}

					MonoScript targetScript = types[attribute.targetType];
					// firstScript는 어트리뷰트에서 지정한 타겟 타입이며,
					// secondScript는 해당 어트리뷰트를 보유한 타입.
					ScriptExecutionOrderDependency dependency = new ScriptExecutionOrderDependency() { AttributeTargetType = targetScript, AttributeMasterType = script, orderDelta = attribute.orderIncrease };
					list.Add(dependency);
				}
			}

			if (hasExecuteBeforeAttribute)
			{
				if(hasExecutionOrderAttribute)
				{
					Debug.LogError(string.Format("Script {0} has both [ExecutionOrder] and [ExecuteBefore] attributes. Ignoring the [ExecuteBefore] attribute.", script.name), script);
					continue;
				}

				if(hasExecuteAfterAttribute)
				{
					Debug.LogError(string.Format("Script {0} has both [ExecuteAfter] and [ExecuteBefore] attributes. Ignoring the [ExecuteBefore] attribute.", script.name), script);
					continue;
				}

				var attributes = (ExecuteBeforeAttribute[])Attribute.GetCustomAttributes(type, typeof(ExecuteBeforeAttribute));
				foreach(var attribute in attributes)
				{
					if(attribute.orderDecrease < 0)
					{
						Debug.LogError(string.Format("Script {0} has an [ExecuteBefore] attribute with a negative orderDecrease. Use the [ExecuteAfter] attribute instead. Ignoring this [ExecuteBefore] attribute.", script.name), script);
						continue;
					}

					if(!attribute.targetType.IsSubclassOf(typeof(MonoBehaviour)) && !attribute.targetType.IsSubclassOf(typeof(ScriptableObject)))
					{
						Debug.LogError(string.Format("Script {0} has an [ExecuteBefore] attribute with targetScript={1} which is not a MonoBehaviour nor a ScriptableObject. Ignoring this [ExecuteBefore] attribute.", script.name, attribute.targetType.Name), script);
						continue;
					}

					MonoScript targetScript = types[attribute.targetType];
					
					// 우선도에 대해 before 값을 가지므로, orderDelta 값을 음수로 세트하는 것을 확인할 수 있다.
					ScriptExecutionOrderDependency dependency = new ScriptExecutionOrderDependency() { AttributeTargetType = targetScript, AttributeMasterType = script, orderDelta = -attribute.orderDecrease };
					list.Add(dependency);
				}
			}
		}

		return list;
	}

	/// <summary>
	/// 입력받은 컬렉션에서 ExecutionOrderAttribute 어트리뷰트를 가지는 타입을 튜플 리스트로 리턴하는 메서드
	/// </summary>
	private static List<ScriptExecutionOrderDefinition> GetExecutionOrderDefinitions(Dictionary<Type, MonoScript> types)
	{
		var list = new List<ScriptExecutionOrderDefinition>();

		foreach(var kvp in types)
		{
			var type = kvp.Key;
			var script = kvp.Value;
			if(Attribute.IsDefined(type, typeof(ExecutionOrderAttribute)))
			{
				var attribute = (ExecutionOrderAttribute)Attribute.GetCustomAttribute(type, typeof(ExecutionOrderAttribute));
				ScriptExecutionOrderDefinition definition = new ScriptExecutionOrderDefinition() { script = script, order = attribute.order };
				list.Add(definition);
			}
		}

		return list;
	}

	/// <summary>
	/// definitions의 ExecutionOrderAttribute 어트리뷰트에 정의된 초기화 우선도 값을 컬렉션으로 초기화 해서 리턴하는 메서드
	/// 딱히 정의된 우선도가 없는 경우 기존의 유니티 에디터에서 해당 스크립트에 지정한 우선도 값을 초기화 해서 리턴한다.
	/// </summary>
	private static Dictionary<MonoScript, int> GetInitalExecutionOrder(List<ScriptExecutionOrderDefinition> definitions, List<MonoScript> graphRoots)
	{
		var orders = new Dictionary<MonoScript, int>();
		foreach(var definition in definitions)
		{
			var script = definition.script;
			var order = definition.order;
			orders.Add(script, order);
		}

		// 다른 노드를 의존하지 않는 루트 노드 중에서, 초기화 순서도 지정받지 않은 스크립트 클래스는
		// 다른 의존하는 노드의 기준점이 되어야 하기 때문에 유니티 에디터로부터 자동을 지정받은 초기화 값을 가진다.
		foreach(var script in graphRoots)
		{
			if(!orders.ContainsKey(script))
			{
				int order = MonoImporter.GetExecutionOrder(script);
				orders.Add(script, order);
			}
		}

		return orders;
	}

	/// <summary>
	/// 입력받은 컬렉션의 [스크립트 타입, 초기화 우선도]에 따라 유니티 에디터의 초기화 순서를 갱신하는 메서드
	/// </summary>
	private static void UpdateExecutionOrder(Dictionary<MonoScript, int> orders)
	{
		bool startedEdit = false;

		foreach(var kvp in orders)
		{
			var script = kvp.Key;
			var order = kvp.Value;

			// 지정한 스크립트의 우선도가 변경되어야 하는 경우
			if(MonoImporter.GetExecutionOrder(script) != order)
			{
				// 유니터 에디터 변경 플래그를 세운다.
				if(!startedEdit)
				{
					AssetDatabase.StartAssetEditing();
					startedEdit = true;
				}
				MonoImporter.SetExecutionOrder(script, order);
			}
		}

		// 유니티 에디터 변경 플래그를 해제한다.
		if(startedEdit)
		{
			AssetDatabase.StopAssetEditing();
		}
	}

	#endregion

	#region <Callbacks>

	/// <summary>
	/// 유니티 엔진에 의해 호출되는 콜백.
	/// 스크립트 어트리뷰트를 읽고 초기화 순서를 갱신한다.
	/// </summary>
	[DidReloadScripts]
	private static void OnDidReloadScripts()
	{
		// 현재 추적중인 유니티 에디터의 클래스 중에서 유니티 스크립트 클래스를 [타입, 스크립트 객체] 컬렉션으로 가져온다.
		var types = GetTypeDictionary();
		
		// 가져온 타입 컬렉션으로부터 '초기화 순서'가 지정된 스크립트 객체를 리스트로 가져온다.
		var definitions = GetExecutionOrderDefinitions(types);
		
		// 가져온 타입 컬렉션으로 부터 '초기화 종속'이 지정된 스크립트 객체를 리스트로 가져온다.
		var dependencies = GetExecutionOrderDependencies(types);
		
		// 초기화 순서 및 초기화 종속으로부터 스크립트 초기화 순서에 관한 연결 그래프를 기술하는 컬렉션을 생성한다.
		var graph = Graph.Create(definitions, dependencies);

		// 초기화 우선도에 루프가 탐지된 경우, 우선도를 선정할 수 없으므로 에러를 뱉는다.
		if(Graph.IsCyclic(graph))
		{
			Debug.LogError("Circular script execution order definitions");
			return;
		}

		// 생성된 그래프에서 다른 노드로의 의존성을 가지지 않는 루트 노드들의 리스트를 가져온다.
		var roots = Graph.GetRoots(graph);
		
		// 각 스크립트 타입에 맞는 초기화 순서값을 컬렉션으로 가져온다.
		var orders = GetInitalExecutionOrder(definitions, roots);
		
		Graph.PropagateValues(graph, orders);

		UpdateExecutionOrder(orders);
	}

	#endregion
}
