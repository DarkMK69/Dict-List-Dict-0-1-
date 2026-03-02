using System;
using CustomCollections;

namespace ListExample
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Демонстрация MyList<T> ===\n");

            // 1. Создание списка
            Console.WriteLine("1. Создание и добавление элементов:");
            MyList<int> numbers = new MyList<int>();
            
            numbers.Add(10);
            numbers.Add(20);
            numbers.Add(30);
            numbers.Add(40);
            numbers.Add(50);
            
            Console.WriteLine($"Количество элементов: {numbers.Count}");
            Console.WriteLine($"Емкость: {numbers.Capacity}");
            
            // 2. Доступ по индексу
            Console.WriteLine("\n2. Доступ по индексу:");
            Console.WriteLine($"numbers[0] = {numbers[0]}");
            Console.WriteLine($"numbers[2] = {numbers[2]}");
            
            // Изменение элемента
            numbers[1] = 25;
            Console.WriteLine($"После изменения: numbers[1] = {numbers[1]}");
            
            // 3. Поиск элементов
            Console.WriteLine("\n3. Поиск элементов:");
            Console.WriteLine($"Содержит 30: {numbers.Contains(30)}");
            Console.WriteLine($"Индекс 40: {numbers.IndexOf(40)}");
            Console.WriteLine($"Последний индекс 30: {numbers.LastIndexOf(30)}");
            
            // 4. Удаление элементов
            Console.WriteLine("\n4. Удаление элементов:");
            Console.WriteLine($"До удаления Count: {numbers.Count}");
            numbers.Remove(30);
            Console.WriteLine($"После удаления 30 Count: {numbers.Count}");
            
            numbers.RemoveAt(0);
            Console.WriteLine($"После удаления по индексу 0 Count: {numbers.Count}");
            
            // 5. Вставка элемента
            Console.WriteLine("\n5. Вставка элемента:");
            numbers.Insert(1, 35);
            Console.WriteLine($"После вставки 35 на позицию 1:");
            PrintList(numbers);
            
            // 6. Добавление коллекции
            Console.WriteLine("\n6. Добавление коллекции:");
            int[] newNumbers = { 60, 70, 80 };
            numbers.AddRange(newNumbers);
            Console.WriteLine($"После AddRange Count: {numbers.Count}");
            
            // 7. Поиск по условию
            Console.WriteLine("\n7. Поиск по условию:");
            var evenNumbers = numbers.FindAll(x => x % 2 == 0);
            Console.WriteLine("Четные числа:");
            PrintList(evenNumbers);
            
            // 8. Использование foreach
            Console.WriteLine("\n8. Перебор с помощью foreach:");
            foreach (var number in numbers)
            {
                Console.Write(number + " ");
            }
            Console.WriteLine();
            
            // 9. Преобразование в массив
            Console.WriteLine("\n9. Преобразование в массив:");
            int[] array = numbers.ToArray();
            Console.WriteLine($"Массив длиной: {array.Length}");
            
            // 10. Очистка списка
            Console.WriteLine("\n10. Очистка списка:");
            numbers.Clear();
            Console.WriteLine($"После Clear Count: {numbers.Count}");
            
            Console.WriteLine("\n=== Демонстрация завершена ===");
        }
        
        static void PrintList<T>(MyList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine($"  [{i}] = {list[i]}");
            }
        }
    }
}