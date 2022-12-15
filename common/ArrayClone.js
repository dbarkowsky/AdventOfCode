export default clone = (items) => items.map(item => Array.isArray(item) ? clone(item) : item);
