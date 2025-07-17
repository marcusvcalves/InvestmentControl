CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    brokerage_percentage DECIMAL NOT NULL
);

CREATE TABLE IF NOT EXISTS assets (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(10) NOT NULL,
    name VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS operations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    asset_id UUID NOT NULL,
    quantity INT NOT NULL,
    unit_price DECIMAL NOT NULL,
    operation_type INT NOT NULL,
    brokerage DECIMAL NOT NULL,
    created_at TIMESTAMPTZ NOT NULL,
    
    CONSTRAINT fk_user_operations
        FOREIGN KEY (user_id)
        REFERENCES users(id),
    
    CONSTRAINT fk_asset_operations
        FOREIGN KEY (asset_id)
        REFERENCES assets(id)
);

CREATE TABLE IF NOT EXISTS quotations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    asset_id UUID NOT NULL,
    unit_price DECIMAL NOT NULL,
    created_at TIMESTAMPTZ NOT NULL,

    CONSTRAINT fk_asset_quotations
        FOREIGN KEY (asset_id)
        REFERENCES assets(id)
);

CREATE TABLE IF NOT EXISTS positions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    asset_id UUID NOT NULL,
    quantity INT NOT NULL,
    medium_price DECIMAL NOT NULL,
    profit_loss DECIMAL NOT NULL,

    CONSTRAINT fk_user_positions
        FOREIGN KEY (user_id)
        REFERENCES users(id),
        
    CONSTRAINT fk_asset_positions
        FOREIGN KEY (asset_id)
        REFERENCES assets(id)
);

CREATE INDEX IF NOT EXISTS idx_assets_code ON assets (code);

CREATE INDEX IF NOT EXISTS idx_operations_user_id ON operations (user_id);
CREATE INDEX IF NOT EXISTS idx_operations_asset_id ON operations (asset_id);
CREATE INDEX IF NOT EXISTS idx_operations_created_at ON operations (created_at);

CREATE INDEX IF NOT EXISTS idx_quotations_asset_id ON quotations (asset_id);
CREATE INDEX IF NOT EXISTS idx_quotations_created_at ON quotations (created_at);

CREATE INDEX IF NOT EXISTS idx_positions_user_id ON positions (user_id);
CREATE INDEX IF NOT EXISTS idx_positions_asset_id ON positions (asset_id);