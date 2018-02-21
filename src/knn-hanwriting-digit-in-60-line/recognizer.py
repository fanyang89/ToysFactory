import os

train_data = [[] for i in range(10)]


def matrix_from_file(path):
    matrix = [([0] * 32) for i in range(32)]
    with open(path, 'r') as f:
        lines = f.readlines()
        for i in range(32):
            for j in range(32):
                matrix[i][j] = int(lines[i][j])
    return matrix


def init_train_data():
    train_data_folder = 'data/train/'
    train_file_path = os.listdir(train_data_folder)
    for path in train_file_path:
        digit = int(path[0])
        matrix = matrix_from_file(train_data_folder + path)
        train_data[digit].append(matrix)


def matrix_distance(A, B):
    sum = 0.0
    for i in range(32):
        for j in range(32):
            sum += (A[i][j] - B[i][j]) ** 2

    return sum ** 0.5


def classify(X, k):
    result = [[str(x), 0.0] for x in range(10)]
    for i in range(10):
        for Y in train_data[i]:
            result[i][1] += matrix_distance(X, Y)
    result.sort(key=lambda r: r[1])
    return result[0:k]


def test():
    test_file_path = os.listdir('data/test/')
    total = len(test_file_path)
    c = 0
    for p in test_file_path:
        X = matrix_from_file('data/test/' + p)
        result = classify(X, 1)
        correct_result = p[0]
        if str(correct_result) == result[0][0]:
            c += 1
        print "Correct %d/%d" % (c, total)


if __name__ == "__main__":
    init_train_data()
    test()
