import Image from 'next/image'
import cn from 'classnames'

import classes from './UserHistoryTabs.module.scss'

type TabItem = {
  label: string
  value: string
  iconPath: string
  iconActivePath: string
}

type Slots = {
  actions?: React.ReactNode
}

type UserHistoryTabsProps = {
  activeTab: string
  tabs: TabItem[]
  slots?: Slots
  onTabChange: (tabValue: string) => void
}

export const UserHistoryTabs = ({ activeTab, tabs, slots, onTabChange }: UserHistoryTabsProps) => {
  return (
    <div className={classes.userHistoryTabs}>
      <ul className={classes.tabsList}>
        {tabs.map(({ label, value, iconPath, iconActivePath }) => {
          const isActive = activeTab === value

          return (
            <li key={value}>
              <button
                type='button'
                className={cn(classes.tabBtn, {
                  [classes.tabBtnActive]: isActive
                })}
                onClick={() => {
                  onTabChange(value)
                }}
              >
                <Image src={iconPath} width={25} height={25} alt={label} />

                <Image
                  className={cn(classes.tabActiveIcon)}
                  src={iconActivePath}
                  width={25}
                  height={25}
                  alt={label}
                />

                <span>{label}</span>
              </button>
            </li>
          )
        })}
      </ul>

      {slots?.actions ? <div className={classes.actions}>{slots.actions}</div> : null}
    </div>
  )
}
