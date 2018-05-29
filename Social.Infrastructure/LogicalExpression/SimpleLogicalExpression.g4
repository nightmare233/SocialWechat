grammar SimpleLogicalExpression;
 
@parser::members
{
    protected const int EOF = Eof;
}
 
@lexer::members
{
    protected const int EOF = Eof;
    protected const int HIDDEN = Hidden;
}
 
/*
 * Parser Rules
 */
 
prog: expr+ ;
 
expr : expr 'AND' expr  # AND
     | expr 'OR' expr  # OR
     | INT                  # int
     | '(' expr ')'         # parens
     ;
 
/*
 * Lexer Rules
 */
INT : [0-9]+;
AND : 'AND';
OR : 'OR';
WS
    :   (' ' | '\r' | '\n') -> channel(HIDDEN)
    ;