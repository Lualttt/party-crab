import re

default_markups = {
    "**": ("<b>",    "</b>"),
    "_":  ("<i>",    "</i>"),
    "*":  ("<i>",    "</i>"),
    "||": ("<mark>", "</mark>")
}

def markup_replace(text, value, start, end):
    return_text = text
    value_count = return_text.count(value)
    escape_count = return_text.count("\\"+value)
    value_count -= escape_count

    if value_count % 2:
        value_count -= 1
    
    value_previous = 0
    for _ in range(int(value_count / 2)):
        value_start = return_text.find(value, value_previous)
        
        return_text = return_text[:value_start] + start + return_text[value_start + len(value):]
        
        value_end = return_text.find(value, value_start + len(value))
        return_text = return_text[:value_end] + end + return_text[value_end + len(value):]
        
        value_previous = value_end + len(value)
        
    return return_text


def markup_color(text):
    value_count = min(text.count("["), text.count("]"))

    if value_count % 2:
        value_count -= 1

    pattern = r"\[(.*?)\]"
    value_amount = len(re.findall(pattern, text))
    return_text = re.sub(pattern, r"<color=\g<1>>", text)
    return_text = return_text + "</color>"*value_amount

    return return_text


def markup(text, markups=default_markups):
    return_text = text
    
    for key, value in markups.items():
        return_text = markup_replace(return_text, key, value[0], value[1])
    
    return_text = markup_color(return_text)
    
    return return_text


if __name__ == "__main__":
    text1 = "normal *italic* _italic_ **bold** ||mark|| [pink]googoo gaga :D"
    print(text1)
    text1 = markup(text1)
    print(text1)
