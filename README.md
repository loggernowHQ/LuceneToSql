
# LuceneToManticoreSql
This package is used to convert lucene to manticore sql.



## Features

This package only has one static method which you can access by using 

- getWhereClause
```
LuceneToSql.getWhereClause(<luceneQuery>) //resturns strign containing where clause part of manticore sql
```

Few examples of query translations are as below

- Lucene Query is -> 
```
(title:"foo bar" AND body:"quick fox") OR title:fox
```
Manticore where clause is -> 
```
(log_json.title='foo bar' AND log_json.body='quick fox') OR log_json.title='fox'
```

- Lucene Query is -> 
```
title:"foo"
```
Manticore where clause is -> 
```
log_json.title='foo'
```

- Lucene Query is -> 
```
title:foo -title:bar
```
Manticore where clause is -> 
```
log_json.title='foo' AND  NOT log_json.title='bar'
```
- Lucene Query is -> 
```
title:foo*
```
Manticore where clause is -> 
```
REGEX(log_json.title,'foo*')
```

- Lucene Query is -> 
```
title:foo*bar
```
Manticore where clause is -> 
```
REGEX(log_json.title,'foo*bar')
```
- Lucene Query is -> 
```
mod_date:[20020101 TO 20030101]
```
Manticore where clause is -> 
```
log_json.mod_date BETWEEN 20020101 AND 20030101
```
- Lucene Query is -> 
```
title:foo
```
Manticore where clause is -> 
```
log_json.title='foo'
```
- Lucene Query is -> 
```
zipcode:7
```
Manticore where clause is -> 
```
log_json.zipcode= 7
```