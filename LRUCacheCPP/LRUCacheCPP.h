#pragma once

#include <string>
#include <map>
#include <list>
#include <mutex>

using namespace std;

namespace LRUCacheLib {

	template <class K, class V>
	class LRUCacheCPP {
	public:
		LRUCacheCPP(size_t Capacity);
		virtual ~LRUCacheCPP();

		size_t Capacity();
		size_t Count();

		V Get(K key);
		void Put(K key, V value);

		bool Remove(K key);
		list<pair<K, V>> ToList();
	private:
		size_t    capacity = 0;
		list<K>   cache;
		map<K, V> items;
		mutex     classpadlock;
	};

}