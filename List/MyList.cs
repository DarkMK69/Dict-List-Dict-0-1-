using System;
using System.Collections;
using System.Collections.Generic;

namespace CustomCollections
{
    /// <summary>
    /// Упрощенная реализация динамического списка (аналог List<T>)
    /// Для учебных целей
    /// </summary>
    /// <typeparam name="T">Тип элементов в списке</typeparam>
    public class MyList<T> : IEnumerable<T>
    {
        // Внутренние поля
        private T[] _items;          // Массив для хранения элементов
        private int _count;          // Текущее количество элементов
        private int _capacity;       // Емкость внутреннего массива
        private int _version;        // Версия для отслеживания изменений

        // Константы
        private const int DefaultCapacity = 4;

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public MyList()
        {
            _capacity = DefaultCapacity;
            _items = new T[_capacity];
            _count = 0;
            _version = 0;
        }

        /// <summary>
        /// Конструктор с указанием начальной емкости
        /// </summary>
        /// <param name="capacity">Начальная емкость списка</param>
        public MyList(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Емкость не может быть отрицательной");

            _capacity = capacity > 0 ? capacity : DefaultCapacity;
            _items = new T[_capacity];
            _count = 0;
            _version = 0;
        }

        /// <summary>
        /// Конструктор из существующей коллекции
        /// </summary>
        /// <param name="collection">Исходная коллекция</param>
        public MyList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            // Пытаемся получить количество элементов для оптимизации
            if (collection is ICollection<T> c)
            {
                _count = c.Count;
                _capacity = _count > 0 ? _count : DefaultCapacity;
                _items = new T[_capacity];
                c.CopyTo(_items, 0);
            }
            else
            {
                _capacity = DefaultCapacity;
                _items = new T[_capacity];
                _count = 0;
                
                // Добавляем элементы по одному
                foreach (var item in collection)
                {
                    Add(item);
                }
            }
            
            _version = 0;
        }

        /// <summary>
        /// Текущее количество элементов в списке
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Емкость внутреннего массива
        /// </summary>
        public int Capacity
        {
            get => _capacity;
            set
            {
                if (value < _count)
                    throw new ArgumentOutOfRangeException(nameof(value), "Емкость не может быть меньше количества элементов");

                if (value != _capacity)
                {
                    if (value > 0)
                    {
                        T[] newItems = new T[value];
                        if (_count > 0)
                        {
                            Array.Copy(_items, 0, newItems, 0, _count);
                        }
                        _items = newItems;
                        _capacity = value;
                    }
                    else
                    {
                        _capacity = DefaultCapacity;
                        _items = new T[_capacity];
                    }
                }
            }
        }

        /// <summary>
        /// Индексатор для доступа к элементам по индексу
        /// </summary>
        /// <param name="index">Индекс элемента</param>
        /// <returns>Элемент по указанному индексу</returns>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count)
                    throw new IndexOutOfRangeException($"Индекс {index} вне диапазона [0, {_count - 1}]");
                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _count)
                    throw new IndexOutOfRangeException($"Индекс {index} вне диапазона [0, {_count - 1}]");
                _items[index] = value;
                _version++;
            }
        }

        /// <summary>
        /// Добавляет элемент в конец списка
        /// </summary>
        /// <param name="item">Добавляемый элемент</param>
        public void Add(T item)
        {
            if (_count == _capacity)
            {
                EnsureCapacity(_count + 1);
            }
            
            _items[_count] = item;
            _count++;
            _version++;
        }

        /// <summary>
        /// Добавляет коллекцию элементов в конец списка
        /// </summary>
        /// <param name="collection">Коллекция элементов</param>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (collection is ICollection<T> c)
            {
                int count = c.Count;
                if (count > 0)
                {
                    EnsureCapacity(_count + count);
                    c.CopyTo(_items, _count);
                    _count += count;
                    _version++;
                }
            }
            else
            {
                foreach (var item in collection)
                {
                    Add(item);
                }
            }
        }

        /// <summary>
        /// Вставляет элемент в указанную позицию
        /// </summary>
        /// <param name="index">Позиция для вставки</param>
        /// <param name="item">Вставляемый элемент</param>
        public void Insert(int index, T item)
        {
            if (index < 0 || index > _count)
                throw new ArgumentOutOfRangeException(nameof(index), $"Индекс должен быть в диапазоне [0, {_count}]");

            if (_count == _capacity)
            {
                EnsureCapacity(_count + 1);
            }

            if (index < _count)
            {
                Array.Copy(_items, index, _items, index + 1, _count - index);
            }

            _items[index] = item;
            _count++;
            _version++;
        }

        /// <summary>
        /// Удаляет первый найденный элемент
        /// </summary>
        /// <param name="item">Удаляемый элемент</param>
        /// <returns>true если элемент был удален, иначе false</returns>
        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Удаляет элемент по указанному индексу
        /// </summary>
        /// <param name="index">Индекс удаляемого элемента</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count)
                throw new IndexOutOfRangeException($"Индекс {index} вне диапазона [0, {_count - 1}]");

            _count--;
            if (index < _count)
            {
                Array.Copy(_items, index + 1, _items, index, _count - index);
            }
            
            _items[_count] = default(T); // Очищаем последнюю позицию
            _version++;
        }

        /// <summary>
        /// Удаляет все элементы, удовлетворяющие условию
        /// </summary>
        /// <param name="match">Условие для удаления</param>
        /// <returns>Количество удаленных элементов</returns>
        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            int freeIndex = 0;
            
            // Находим первый элемент для удаления
            while (freeIndex < _count && !match(_items[freeIndex]))
                freeIndex++;

            if (freeIndex >= _count)
                return 0;

            int current = freeIndex + 1;
            while (current < _count)
            {
                // Находим элемент, который нужно сохранить
                while (current < _count && match(_items[current]))
                    current++;

                if (current < _count)
                {
                    _items[freeIndex++] = _items[current++];
                }
            }

            int removedCount = _count - freeIndex;
            Array.Clear(_items, freeIndex, removedCount);
            _count = freeIndex;
            _version++;
            
            return removedCount;
        }

        /// <summary>
        /// Удаляет все элементы из списка
        /// </summary>
        public void Clear()
        {
            if (_count > 0)
            {
                Array.Clear(_items, 0, _count);
                _count = 0;
            }
            _version++;
        }

        /// <summary>
        /// Проверяет, содержит ли список указанный элемент
        /// </summary>
        /// <param name="item">Искомый элемент</param>
        /// <returns>true если элемент найден, иначе false</returns>
        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        /// <summary>
        /// Ищет индекс первого вхождения элемента
        /// </summary>
        /// <param name="item">Искомый элемент</param>
        /// <returns>Индекс элемента или -1 если не найден</returns>
        public int IndexOf(T item)
        {
            for (int i = 0; i < _count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Ищет индекс первого вхождения элемента, начиная с указанной позиции
        /// </summary>
        /// <param name="item">Искомый элемент</param>
        /// <param name="startIndex">Начальный индекс поиска</param>
        /// <returns>Индекс элемента или -1 если не найден</returns>
        public int IndexOf(T item, int startIndex)
        {
            if (startIndex < 0 || startIndex > _count)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            for (int i = startIndex; i < _count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Ищет последнее вхождение элемента
        /// </summary>
        /// <param name="item">Искомый элемент</param>
        /// <returns>Индекс элемента или -1 если не найден</returns>
        public int LastIndexOf(T item)
        {
            for (int i = _count - 1; i >= 0; i--)
            {
                if (EqualityComparer<T>.Default.Equals(_items[i], item))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Находит первый элемент, удовлетворяющий условию
        /// </summary>
        /// <param name="match">Условие поиска</param>
        /// <returns>Найденный элемент или default(T) если не найден</returns>
        public T Find(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            for (int i = 0; i < _count; i++)
            {
                if (match(_items[i]))
                    return _items[i];
            }
            return default(T);
        }

        /// <summary>
        /// Находит все элементы, удовлетворяющие условию
        /// </summary>
        /// <param name="match">Условие поиска</param>
        /// <returns>Список найденных элементов</returns>
        public MyList<T> FindAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));

            MyList<T> result = new MyList<T>();
            for (int i = 0; i < _count; i++)
            {
                if (match(_items[i]))
                    result.Add(_items[i]);
            }
            return result;
        }

        /// <summary>
        /// Выполняет действие для каждого элемента списка
        /// </summary>
        /// <param name="action">Выполняемое действие</param>
        public void ForEach(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            for (int i = 0; i < _count; i++)
            {
                action(_items[i]);
            }
        }

        /// <summary>
        /// Преобразует список в массив
        /// </summary>
        /// <returns>Массив элементов</returns>
        public T[] ToArray()
        {
            T[] array = new T[_count];
            Array.Copy(_items, 0, array, 0, _count);
            return array;
        }

        /// <summary>
        /// Копирует элементы списка в массив
        /// </summary>
        /// <param name="array">Целевой массив</param>
        /// <param name="arrayIndex">Начальный индекс в целевом массиве</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < _count)
                throw new ArgumentException("Целевой массив слишком мал");

            Array.Copy(_items, 0, array, arrayIndex, _count);
        }

        /// <summary>
        /// Уменьшает емкость до текущего количества элементов
        /// </summary>
        public void TrimExcess()
        {
            int threshold = (int)(_capacity * 0.9);
            if (_count < threshold)
            {
                Capacity = _count;
            }
        }

        /// <summary>
        /// Обеспечивает минимальную емкость
        /// </summary>
        /// <param name="min">Минимальная необходимая емкость</param>
        private void EnsureCapacity(int min)
        {
            if (_capacity < min)
            {
                int newCapacity = _capacity == 0 ? DefaultCapacity : _capacity * 2;
                
                // Проверка на переполнение
                if (newCapacity > Array.MaxLength)
                    newCapacity = Array.MaxLength;
                    
                if (newCapacity < min)
                    newCapacity = min;
                    
                Capacity = newCapacity;
            }
        }

        /// <summary>
        /// Возвращает перечислитель для foreach
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Вложенный класс перечислителя
        /// </summary>
        private class Enumerator : IEnumerator<T>
        {
            private readonly MyList<T> _list;
            private readonly int _version;
            private int _index;
            private T _current;

            public Enumerator(MyList<T> list)
            {
                _list = list;
                _version = list._version;
                _index = 0;
                _current = default(T);
            }

            public T Current => _current;

            object IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _list._count + 1)
                        throw new InvalidOperationException("Перечислитель в недопустимом состоянии");
                    return Current;
                }
            }

            public bool MoveNext()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Коллекция была изменена во время перечисления");

                if (_index < _list._count)
                {
                    _current = _list._items[_index];
                    _index++;
                    return true;
                }

                _index = _list._count + 1;
                _current = default(T);
                return false;
            }

            public void Reset()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Коллекция была изменена во время перечисления");

                _index = 0;
                _current = default(T);
            }

            public void Dispose()
            {
                // Ничего не делаем, так как нет неуправляемых ресурсов
            }
        }
    }
}