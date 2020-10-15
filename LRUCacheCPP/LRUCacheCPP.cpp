#include "pch.h"
#include "LRUCacheCPP.h"

#include <iostream>
#include <string>
#include <map>
#include <list>
#include <mutex>

using namespace std;

namespace LRUCacheLib {

	template<class K, class V>
	LRUCacheLib::LRUCacheCPP<K, V>::LRUCacheCPP(size_t Capacity) {
		capacity = Capacity;
	}

	template <class K, class V>
	LRUCacheCPP<K, V>::~LRUCacheCPP() {
		const lock_guard<mutex> lock(classpadlock);
		items.clear();
		cache.clear();
	}

	template <class K, class V>
	size_t LRUCacheCPP<K, V>::Capacity() { return capacity; }

	template <class K, class V>
	size_t LRUCacheCPP<K, V>::Count() { return cache.size(); }

	template <class K, class V>
	V LRUCacheCPP<K, V>::Get(K key)
	{
		const lock_guard<mutex> lock(classpadlock);

		auto it = items.find(key);
		if (it != items.end())
		{
			cache.remove(key);
			cache.push_back(key);
			return it->second;
		}

		return nullptr;
	}

	template <class K, class V>
	bool LRUCacheCPP<K, V>::Remove(K key)
	{
		const lock_guard<mutex> lock(classpadlock);

		auto it = items.find(key);
		if (it != items.end())
		{
			cache.remove(key);
			items.erase(key);
			return true;
		}

		return false;
	}

	template <class K, class V>
	void LRUCacheCPP<K, V>::Put(K key, V value)
	{
		const lock_guard<mutex> lock(classpadlock);

		auto it = items.find(key);
		if (it != items.end())
		{
			cache.remove(key);
			cache.push_back(key);
			it->second = value;
			return;
		}

		//Add a new node
		cache.push_back(key);
		items.insert(pair<K, V>(key, value));

		// Check size of cache
		if (Count() > Capacity())
		{
			int tmp_key = cache.front();
			items.erase(tmp_key);
			cache.pop_front();
		}
	}

	template <class K, class V>
	list<pair<K, V>> LRUCacheCPP<K, V>::ToList()
	{
		const lock_guard<mutex> lock(classpadlock);

		list<pair<K, V>>  list(0);
		for (auto x : cache)
		{
			auto it = items.find(x);
			list.push_back(pair<int, string>(it->first, it->second));
		}

		return list;
	}

} /* namespace LRUCacheLib */
