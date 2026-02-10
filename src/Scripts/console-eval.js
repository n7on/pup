(expression) => {
    try {
        const result = eval(expression);

        const serialize = (val, depth) => {
            if (depth === undefined) depth = 0;
            if (depth > 5) return JSON.stringify('[Max depth]');
            if (val === undefined) return JSON.stringify('[undefined]');
            if (val === null) return 'null';
            if (typeof val === 'function') return JSON.stringify('[Function: ' + (val.name || 'anonymous') + ']');
            if (typeof val === 'symbol') return JSON.stringify(val.toString());

            if (val instanceof Element) {
                let desc = '<' + val.tagName.toLowerCase();
                if (val.id) desc += ' id="' + val.id + '"';
                if (val.className) desc += ' class="' + val.className + '"';
                desc += '>';
                return JSON.stringify(desc);
            }

            if (val instanceof NodeList || val instanceof HTMLCollection) {
                const items = Array.from(val).slice(0, 5).map(function(el) {
                    let tag = el.tagName ? el.tagName.toLowerCase() : 'node';
                    if (el.id) tag += '#' + el.id;
                    return tag;
                });
                let desc = 'NodeList(' + val.length + ') [' + items.join(', ');
                if (val.length > 5) desc += ', ...+' + (val.length - 5) + ' more';
                desc += ']';
                return JSON.stringify(desc);
            }

            if (val instanceof Window) return JSON.stringify('[Window]');
            if (val instanceof Document) return JSON.stringify('[Document]');
            if (val instanceof Promise) return JSON.stringify('[Promise]');

            if (Array.isArray(val)) {
                if (val.length === 0) return '[]';
                const items = val.slice(0, 20).map(function(v) { return serialize(v, depth + 1); });
                let arrStr = '[' + items.join(',');
                if (val.length > 20) arrStr += ',"...+' + (val.length - 20) + ' more"';
                return arrStr + ']';
            }

            if (typeof val === 'object') {
                try {
                    const keys = Object.keys(val);
                    if (keys.length === 0) return '{}';
                    const pairs = keys.slice(0, 20).map(function(k) {
                        return JSON.stringify(k) + ':' + serialize(val[k], depth + 1);
                    });
                    let objStr = '{' + pairs.join(',');
                    if (keys.length > 20) objStr += ',"...+' + (keys.length - 20) + ' more"';
                    return objStr + '}';
                } catch (e) {
                    return JSON.stringify('[Object]');
                }
            }

            if (typeof val === 'string') {
                if (val.length > 500) return JSON.stringify(val.substring(0, 500) + '...(' + val.length + ' chars)');
                return JSON.stringify(val);
            }

            if (typeof val === 'number' || typeof val === 'boolean') {
                return String(val);
            }

            return JSON.stringify(String(val));
        };

        return serialize(result);
    } catch (e) {
        return JSON.stringify({__error: e.message});
    }
}
