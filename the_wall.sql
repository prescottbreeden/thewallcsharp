CREATE TABLE users 
	(
		id INT AUTO_INCREMENT PRIMARY KEY,
		first_name VARCHAR(50) NOT NULL,
		last_name VARCHAR(50) NOT NULL,
		email VARCHAR(100) NOT NULL,
		password VARCHAR(100) NOT NULL,
		created_at DATETIME DEFAULT NOW(),
		updated_at DATETIME DEFAULT NOW()
	);

CREATE TABLE posts
	(
		id INT AUTO_INCREMENT PRIMARY KEY,
		content TEXT NOT NULL,
		user_id INT NOT NULL,
		created_at DATETIME DEFAULT NOW(),
		updated_at DATETIME DEFAULT NOW(),
		FOREIGN KEY(user_id) REFERENCES users(id)
	);

CREATE TABLE comments
	(
		id INT AUTO_INCREMENT PRIMARY KEY,
		content TEXT NOT NULL,
		user_id INT NOT NULL,
		post_id INT NOT NULL,
		created_at DATETIME DEFAULT NOW(),
		updated_at DATETIME DEFAULT NOW(),
		FOREIGN KEY(post_id) REFERENCES posts(id),
		FOREIGN KEY(user_id) REFERENCES users(id)
	);
