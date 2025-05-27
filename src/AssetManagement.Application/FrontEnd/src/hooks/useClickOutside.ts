import { useEffect } from "react";

/**
 * Use click outside (prevent click outside of the element)
 * @param {React.RefObject<HTMLElement>} ref - The ref of the element
 * @param {() => void} handler - The handler to call when the click is outside
 * @description Use click outside to call a handler when the click is outside the element
 * @technique {useEffect} -> Add event listener to the document
 */
function useClickOutside(ref: React.RefObject<HTMLElement>, handler: () => void) {
  useEffect(() => {
    function handleClick(event: MouseEvent) {
      if (ref.current && !ref.current.contains(event.target as Node)) {
        handler();
      }
    }
    document.addEventListener('mousedown', handleClick);
    return () => document.removeEventListener('mousedown', handleClick);
  }, [ref, handler]);
}

export default useClickOutside;