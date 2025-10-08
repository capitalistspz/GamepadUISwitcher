namespace UI;

public static class Utils
{
    public static void Append<T>(ref T[] arr, T newElem)
    {
        var newArr = new T[arr.Length + 1];
        for (var i = 0; i < arr.Length; ++i)
        {
            newArr[i] = arr[i];
        }
        newArr[arr.Length] = newElem;
        arr = newArr;
    }

    public static void InsertAfter<T>(ref T[] arr, T existingElem, T newElem)
    {
        var newArr = new T[arr.Length + 1];
        var i = 0;
        for (; i < arr.Length; ++i)
        {
            var elem = arr[i];
            newArr[i] = elem;
            if (elem != null && elem.Equals(existingElem))
            {
                break;
            }
        }
        if (i == arr.Length)
            return;
        newArr[++i] = newElem;
        for (; i < arr.Length; ++i)
        {
            newArr[i + 1] = arr[i];
        }

        arr = newArr;
    }

}