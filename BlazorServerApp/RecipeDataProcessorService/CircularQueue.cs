using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorServerApp.proccessService
{
    public class CircularQueue<T>
    {
        private T[] _queueItems;
        private int _front;
        private int _rear;
        private int _maximumQueueSize;
        private int _itemsInQueue;


        public CircularQueue(int queueSize)
        {
            _maximumQueueSize = queueSize;
            _queueItems = new T[_maximumQueueSize] ;
            _front = 0;
            _rear = -1;
            _itemsInQueue = 0;
        }

        public void EnqueueItem(T item)
        {
            if(!HasCapacity())
            {
                throw new StackOverflowException();
            }
            else
            {
                _rear = (_rear + 1) % _maximumQueueSize;
                _queueItems[_rear] = item;
                _itemsInQueue++;
            }
        }

        public T DequeueItem()
        {
            if(QueueIsEmpty())
            {
                throw new Exception("No Items in queue");
            }
            else
            {
                _itemsInQueue--;
                int oldQueuePosition = _front;
                _front = (_front + 1) % _maximumQueueSize;
                return _queueItems[oldQueuePosition];
               
            }
        }

        public int GetCapacity()
        {
            return _maximumQueueSize- _itemsInQueue;
        }
        public bool HasCapacity()
        {
            return _itemsInQueue < _maximumQueueSize;
        }

        public bool QueueIsEmpty()
        {
            return _itemsInQueue == 0;
        }

        public string PrintQueue()
        {
            string text = "";
            int i = 0;
            int j = 0;

            if (_itemsInQueue == 0)
            {
                return "Queue is empty"; ;            
            }
            else
            {
                for (i = _front; j < _itemsInQueue;)
                {
                    text +=("\tItem[" + (i + 1) + "]: " + _queueItems[i]);

                    i = (i + 1) % _maximumQueueSize;
                    j++;

                }
            }
            return text;
        }
    }
}
