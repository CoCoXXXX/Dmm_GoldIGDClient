using System.Collections.Generic;
using UnityEngine;

namespace Dmm.Widget
{
    public abstract class PageableItemList<T> : MonoBehaviour
    {
        #region page

        private float _pageChangeTime = 0f;

        public void InvalidatePage()
        {
            _pageChangeTime = Time.time;
        }

        private int _currentPage = 1;

        public int CurrentPage()
        {
            return _currentPage;
        }

        public int CurrentPageStartIndex()
        {
            return PageSize() * (CurrentPage() - 1);
        }

        public int TotalPageCount()
        {
            var pageSize = PageSize();
            if (pageSize <= 0)
                return 0;

            var dataCount = DataCount();
            return dataCount / pageSize + (dataCount % pageSize > 0 ? 1 : 0);
        }

        public bool HasNextPage()
        {
            return CurrentPage() * PageSize() < DataCount();
        }

        public bool HasPreviousPage()
        {
            return CurrentPage() > 1;
        }

        public void GoToNextPage()
        {
            if (!HasNextPage())
                return;

            SetSelectSlot(0);
            _currentPage++;
            _pageChangeTime = Time.time;
        }

        public void GoToPreviousPage()
        {
            if (!HasPreviousPage())
                return;

            SetSelectSlot(0);

            _currentPage--;
            if (_currentPage < 1)
                _currentPage = 1;

            _pageChangeTime = Time.time;
        }

        public int PageDataCount()
        {
            var pageSize = PageSize();
            var startIndex = CurrentPageStartIndex();
            var dataCount = DataCount();

            if (startIndex >= dataCount)
            {
                return 0;
            }

            if (pageSize * _currentPage <= dataCount)
            {
                return pageSize;
            }

            return dataCount % pageSize;
        }

        public T GetPageData(int index)
        {
            var realIndex = CurrentPageStartIndex() + index;
            if (realIndex < 0 || realIndex >= DataCount())
                return default(T);

            return GetData(realIndex);
        }

        #endregion

        #region Unity方法

        public void OnDisable()
        {
            foreach (var item in _itemList)
            {
                Destroy(item.gameObject);
            }

            _itemList.Clear();

            foreach (var item in _idleItemCache)
            {
                Destroy(item.gameObject);
            }

            _idleItemCache.Clear();

            foreach (var div in _dividerList)
            {
                Destroy(div.gameObject);
            }

            _dividerList.Clear();

            foreach (var div in _dividerCache)
            {
                Destroy(div.gameObject);
            }

            _dividerCache.Clear();

            RefreshTime = 0;
        }

        public void Update()
        {
            var dataTime = DataUpdateTime();
            var pageRefresh = RefreshTime < _pageChangeTime;
            var dataRefresh = RefreshTime < dataTime;

            if (pageRefresh || dataRefresh)
            {
                RefreshTime = Mathf.Max(_pageChangeTime, dataTime);
                RefreshContent(pageRefresh, dataRefresh);
            }

            OnUpdate();
        }

        public void RefreshContent(bool pageRefresh, bool dataRefresh)
        {
            if (dataRefresh)
            {
                BeforeRefresh();
            }

            BeforePageRefresh();
            RefreshContent();

            if (dataRefresh)
            {
                AfterRefresh();
            }
        }

        #endregion

        #region 刷新事件

        /// <summary>
        /// Update事件。
        /// </summary>
        public virtual void OnUpdate()
        {
        }

        /// <summary>
        /// 在刷新内容之前调用。
        /// </summary>
        public virtual void BeforeRefresh()
        {
        }

        /// <summary>
        /// 页数切换之前调用。
        /// </summary>
        public virtual void BeforePageRefresh()
        {
        }

        /// <summary>
        /// 在刷新内容之后调用。
        /// </summary>
        public virtual void AfterRefresh()
        {
        }

        /// <summary>
        /// 在绑定内容之前调用。
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <param name="data"></param>
        public virtual void BeforeBindData(int index, Item<T> item, T data)
        {
        }

        #endregion

        /// <summary>
        /// 列表内容的parent。
        /// </summary>
        public RectTransform ContentParent;

        /// <summary>
        /// 槽位列表。
        /// </summary>
        private readonly List<Item<T>> _itemList = new List<Item<T>>();

        /// <summary>
        /// 槽位的缓存。
        /// </summary>
        private readonly Queue<Item<T>> _idleItemCache = new Queue<Item<T>>();

        /// <summary>
        /// 分割线列表。
        /// dividerList[0]是itemList[0]后面的那个分割线。
        /// dividerList.Count == itemList.Count - 1
        /// </summary>
        private readonly List<GameObject> _dividerList = new List<GameObject>();

        /// <summary>
        /// 分割线的缓存。
        /// </summary>
        private readonly Queue<GameObject> _dividerCache = new Queue<GameObject>();

        /// <summary>
        /// 当前选中的slot。
        /// </summary>
        private int _selectedSlot;

        public int CurrentSelectedSlot
        {
            get { return _selectedSlot; }
        }

        /// <summary>
        /// 上一次刷新内容的时间。
        /// </summary>
        public float RefreshTime { get; private set; }

        /// <summary>
        /// 手动设置内容刷新时间。
        /// </summary>
        /// <param name="time"></param>
        public void SetRefreshTime(float time)
        {
            RefreshTime = time;
        }

        /// <summary>
        /// 刷新列表的内容。
        /// </summary>
        public void RefreshContent()
        {
            // 既然分页了，那么每页的大小就是Slot的Count了。
            var slotCount = PageSize();
            var dataCount = PageDataCount();

            if (slotCount < dataCount)
                slotCount = dataCount;

            if (_itemList.Count > slotCount)
            {
                // 删除多余的卡槽。
                for (int i = 0, n = _itemList.Count - slotCount; i < n && _itemList.Count > 0; i++)
                {
                    var item = _itemList[_itemList.Count - 1];
                    _itemList.RemoveAt(_itemList.Count - 1);
                    item.Reset(0);
                    SetItemIdle(item);
                }

                if (HasDivider())
                {
                    for (int i = 0, n = _itemList.Count - slotCount; i < n && _dividerList.Count > 0; i++)
                    {
                        // divider需要删除的数量与item需要删除的数量是一致的。
                        var div = _dividerList[_dividerList.Count - 1];
                        _dividerList.RemoveAt(_dividerList.Count - 1);
                        SetDividerIdle(div);
                    }
                }
            }

            for (int i = 0; i < slotCount; i++)
            {
                // 生成Item。
                Item<T> item = null;
                if (i < _itemList.Count)
                {
                    item = _itemList[i];
                }
                else
                {
                    item = GetItem();

                    if (item)
                    {
                        item.transform.SetParent(ContentParent, false);
                        _itemList.Add(item);
                    }
                }

                if (item) item.transform.SetSiblingIndex(i * 2);

                // 生成Divider。
                if (HasDivider() && i < slotCount - 1)
                {
                    GameObject divider = null;
                    if (i < _dividerList.Count)
                    {
                        divider = _dividerList[i];
                    }
                    else
                    {
                        divider = GetDivider();
                        if (divider)
                        {
                            divider.transform.SetParent(ContentParent, false);
                            _dividerList.Add(divider);
                        }
                    }

                    if (divider) divider.transform.SetSiblingIndex(i * 2 + 1);
                }

                // 绑定数据。
                if (i < dataCount)
                {
                    // 有数据的就绑定数据。
                    var data = GetPageData(i);
                    if (item)
                    {
                        BeforeBindData(i, item, data);
                        item.BindData(i, data);
                    }
                }
                else
                {
                    // 没数据的就Reset成空的状态。
                    if (item) item.Reset(i);
                }
            }

            // 刷新item数据的选中状态。
            for (int i = 0; i < _itemList.Count; i++)
            {
                var item = _itemList[i];

                var selected = i == _selectedSlot;
                item.Select(selected);

                if (selected) OnItemSelected(item);
            }
        }

        public Item<T> GetItem(int index)
        {
            if (index < 0 || index >= _itemList.Count)
                return null;

            return _itemList[index];
        }

        public int GetItemCount()
        {
            return _itemList.Count;
        }

        /// <summary>
        /// 获取显示Item的对象。
        /// </summary>
        /// <returns></returns>
        private Item<T> GetItem()
        {
            if (_idleItemCache.Count > 0)
            {
                var item = _idleItemCache.Dequeue();
                if (item && !item.gameObject.activeSelf)
                    item.gameObject.SetActive(true);

                return item;
            }
            else
            {
                var item = CreateItem();
                if (item)
                {
                    var btn = item.GetClickButton();
                    if (btn) btn.onClick.AddListener(() => Select(item));
                }

                return item;
            }
        }

        /// <summary>
        /// item已经无效了。
        /// </summary>
        /// <param name="item"></param>
        private void SetItemIdle(Item<T> item)
        {
            if (!item) return;

            if (item.gameObject.activeSelf)
                item.gameObject.SetActive(false);

            _idleItemCache.Enqueue(item);
        }

        private GameObject GetDivider()
        {
            if (!HasDivider()) return null;

            if (_dividerCache.Count > 0)
            {
                var div = _dividerCache.Dequeue();
                if (div && !div.activeSelf)
                    div.SetActive(true);

                return div;
            }
            else
            {
                return CreateDivider();
            }
        }

        private void SetDividerIdle(GameObject divider)
        {
            if (!divider) return;

            if (divider.activeSelf)
                divider.SetActive(false);

            _dividerCache.Enqueue(divider);
        }

        #region select item

        public void Select(int index)
        {
            if (index < 0 || index >= _itemList.Count) return;

            _selectedSlot = index;
            RefreshSelectState();
            OnItemSelected(_itemList[index]);
        }

        public void Select(Item<T> item)
        {
            _selectedSlot = _itemList.IndexOf(item);
            RefreshSelectState();
            OnItemSelected(item);
        }

        public void SelectEmpty()
        {
            _selectedSlot = -1;
        }

        /// <summary>
        /// 设置当前选中的slot，但是不刷新。
        /// </summary>
        /// <param name="slot"></param>
        public void SetSelectSlot(int slot)
        {
            if (slot < 0 || slot >= _itemList.Count)
                return;

            _selectedSlot = slot;
        }

        /// <summary>
        /// 刷新列表的选中状态。
        /// </summary>
        private void RefreshSelectState()
        {
            for (int i = 0; i < _itemList.Count; i++)
                _itemList[i].Select(i == _selectedSlot);
        }

        #endregion

        #region abstract methods.

        /// <summary>
        /// 返回每页的大小。
        /// </summary>
        /// <returns></returns>
        public abstract int PageSize();

        /// <summary>
        /// 创建Item。
        /// 这里是CreateItem，而不是GetItemPrefab，是为了在创建之后提供一次自定义属性的机会。
        /// </summary>
        /// <returns></returns>
        public abstract Item<T> CreateItem();

        /// <summary>
        /// 是否存在Divider。
        /// </summary>
        /// <returns></returns>
        public abstract bool HasDivider();

        /// <summary>
        /// 创建Divider。
        /// </summary>
        /// <returns></returns>
        public abstract GameObject CreateDivider();

        /// <summary>
        /// 数据更新的时间。
        /// </summary>
        /// <returns></returns>
        public abstract float DataUpdateTime();

        /// <summary>
        /// 数据的个数。
        /// </summary>
        /// <returns></returns>
        public abstract int DataCount();

        /// <summary>
        /// 获取数据对象。
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract T GetData(int index);

        /// <summary>
        /// item被选中的事件。
        /// </summary>
        /// <param name="item"></param>
        public abstract void OnItemSelected(Item<T> item);

        #endregion
    }
}