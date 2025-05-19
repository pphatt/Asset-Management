import { useEffect, useState } from 'react';


/**
 * Debounce a value (prevent too many re-renders)
 * @param {T} value - The value to debounce
 * @param {number} delay - The delay in milliseconds
 * @returns {T} The debounced value
 * @description Debounce a value to avoid too many re-renders
 * @technique {useState} -> Store the debounced value
 * @technique {useEffect} -> Set the debounced value
 */
function useDebounce<T>(value: T, delay: number): T {
    const [debounced, setDebounced] = useState(value);
    useEffect(() => {
        const handler = setTimeout(() => setDebounced(value), delay);
        return () => clearTimeout(handler);
    }, [value, delay]);
    return debounced;
}

export default useDebounce;
