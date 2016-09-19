using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Ransu {

	// 絶対値が min ～ max の乱数を生成する。符号もランダム。.
	public static float	getSignedRandom(float min, float max)
	{
		min = Mathf.Abs(min);
		max = Mathf.Abs(max);
		
		float	value = Random.Range(min, max);

		if(Random.Range(0.0f, 1.0f) > 0.5f) {

			value = -value;
		}

		return(value);
	}

	public static int	selectWithProbability(List<float> probabilities)
	{
		float	sum = 0.0f;
		int		i = 0;
		
		for(i = 0;i < probabilities.Count;i++) {
			
			sum += probabilities[i];
		}
		
		float	rand = Random.Range(0.0f, sum);
		
		sum = 0.0f;
		
		for(i = 0;i < probabilities.Count;i++) {
			
			if(probabilities[i] == 0.0f) {
				
				continue;
			}
			
			sum += probabilities[i];
			
			if(rand < sum) {
				
				break;
			}
		}
		
		return(i);
	}

	// int の配列の中身をランダムな順番に並び替える.
	public static void	randomSort<T>(List<T> values)
	{
		for(int i = 0;i < values.Count - 2;i++) {

			int		i0 = i;
			int		i1 = Random.Range(i + 1, values.Count);
			
			T		tmp = values[i0];
			values[i0] = values[i1];
			values[i1] = tmp;
		}
	}

	// 0 ～ max - 1 の乱数を重複なく count 個選ぶ.
	public static List<int>	getUniqueRandom(int max, int count)
	{
		List<int>	values = new List<int>();

		if(max <= count) {

			count = max - 1;
		}

		for(int i = 0;i < count;i++) {

			int		n = Random.Range(0, max - i);

			for(int j = 0;j < values.Count;j++) {

				if(n >= values[j]) {

					n++;
				}
			}

			values.Add(n);
			values.Sort();
		}

		return(values);
	}
}
