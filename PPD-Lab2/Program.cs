using System;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Threading;

namespace PPD_Lab2
{
    class Program
    {
        private const int VECTOR_SIZE = 3;
        private static float[] _vectorA;
        private static float[] _vectorB;

        private static Mutex _mutex;
        private static bool _consumed;
        private static float _currentProduct;
        
        static void Main(string[] args)
        {
            Initialize();
            StartThreads();
        }

        static void Initialize()
        {
            _vectorA = new float[VECTOR_SIZE] {2, 3, 5 };
            _vectorB = new float[VECTOR_SIZE] {5, 8, 9 };

            _mutex = new Mutex();
            _currentProduct = 0;
            _consumed = true;
        }

        static void StartThreads()
        {
            var producerThread = new Thread(ProduceProducts) {Name = "Producer"};
            var consumerThread = new Thread(ConsumeProducts) {Name = "Consumer"};
            
            producerThread.Start();
            consumerThread.Start();

            producerThread.Join();
            consumerThread.Join();
        }
        
        static void ProduceProducts()
        {
            for (int i = 0; i < VECTOR_SIZE; i++)
            {
                
                _mutex.WaitOne();

                _currentProduct = _vectorA[i] * _vectorB[i];
                Console.WriteLine($"{Thread.CurrentThread.Name}: produced product {_currentProduct}.");
                _consumed = false;
                _mutex.ReleaseMutex();
                
                while (!_consumed)
                {
                    Thread.Sleep(100);
                }
            }
        }

        static void ConsumeProducts()
        {
            var sum = 0f;

            for (int i = 0; i < VECTOR_SIZE; i++)
            {
                while (_consumed)
                {
                    Thread.Sleep(100);
                }
                
                _mutex.WaitOne();
                
                sum += _currentProduct;
                Console.WriteLine($"{Thread.CurrentThread.Name}: consumed product {_currentProduct}.");
                _consumed = true;
                
                _mutex.ReleaseMutex();
            }
            
            Console.WriteLine($"{Thread.CurrentThread.Name}: final product {sum}.");
        }
    }
}